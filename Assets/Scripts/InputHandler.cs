using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    private Color32 availColor = new Color32(52, 255, 59, 161);
    private Color32 unavailColor = new Color32(255, 0, 0, 161);

    public static InputHandler instance;

    private int selectedX = int.MaxValue, selectedZ = int.MaxValue;
    private int mouseOverX = int.MaxValue, mouseOverZ = int.MaxValue;
    private bool selecting = false;
    private int towerCreationID = -1;

    public PlayerInfo player;
    public GameObject ghostObj;
    private Rigidbody playerRigidbody;


    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckInput();
    }

    // General method for checking various inputs
    // Checks raycast at mouse for a hexagon selection on mouse click
    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
            UI_RequestTower(1);


        playerRigidbody.velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            playerRigidbody.velocity += Vector3.forward * Time.fixedDeltaTime * player.maxSpeed;
        if (Input.GetKey(KeyCode.S))
            playerRigidbody.velocity += -Vector3.forward * Time.fixedDeltaTime * player.maxSpeed;
        if (Input.GetKey(KeyCode.D))
            playerRigidbody.velocity += Vector3.right * Time.fixedDeltaTime * player.maxSpeed;
        if (Input.GetKey(KeyCode.A))
            playerRigidbody.velocity += -Vector3.right * Time.fixedDeltaTime * player.maxSpeed;

        if (playerRigidbody.velocity.x != 0 && playerRigidbody.velocity.z != 0)
            playerRigidbody.velocity /= 1.41421f; // sqrt(2)


        HexInfo obj = CheckRaycast();
        if (selecting)
        {
            if (obj != null)
            {
                int x = obj.x;
                int z = obj.z;
                MouseOverHexagon(x, z);
                if (Input.GetMouseButtonDown(0))
                    SelectHexagon(x, z);
                else if (Input.GetMouseButtonDown(1))
                    ClearAll();
                
            }
            else
            {
                ClearMouseOver();
            }
        }
        else
        {
            // check for if object moused over is a tower, so we can show info if necessary
            if(obj != null)
            {
                if(obj.IsOccupied())
                {
                    int x = obj.x;
                    int z = obj.z;
                    MouseOverHexagon(x, z);
                    if (Input.GetMouseButtonDown(0))
                        SelectHexagon(x, z);
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                        ClearSelected();
                    ClearMouseOver();
                }
            }
        }
    }

    private void ClearAll()
    {
        towerCreationID = -1;
        selecting = false;
        ghostObj.SetActive(false);
        ClearMouseOver();
        ClearSelected();
    }

    // Clears the selected variables, sets hexagons back to normal
    private void ClearSelected()
    {
        if(selectedX != int.MaxValue)
        {
            HexInfo oldInfo = ChunkManager.instance.GetHexAt(selectedX, selectedZ);
            oldInfo.setSelected(0);
        }

        selectedX = int.MaxValue;
        selectedZ = int.MaxValue;
    }

    private void ClearMouseOver()
    {
        if (mouseOverX != int.MaxValue)
        {
            HexInfo oldInfo = ChunkManager.instance.GetHexAt(mouseOverX, mouseOverZ);
            oldInfo.setMouseover(false);
        }
        mouseOverX = int.MaxValue;
        mouseOverZ = int.MaxValue;
    }

    // Deals with a left click at x, z
    private void SelectHexagon(int x, int z)
    {
        HexInfo info = ChunkManager.instance.GetHexAt(x, z);
        if (selectedX != int.MaxValue)
        {
            // one is already selected
            ClearSelected();

            // reselect if new tile
            SelectHexagon(x, z);
        }
        else
        {
            // if creating a tower and this is what the player selects
            if (towerCreationID != -1)
            {
                if(!ChunkManager.instance.IsHexOccupied(x, z))
                    TowerManager.instance.CreateTower(new Coordinate(x, z), towerCreationID);

                ClearAll();
                return;
            }


            // one isn't selected yet
            if (info != null)
            {
                info.setSelected(1);
            }
            selectedX = x;
            selectedZ = z;
        }
    }

    // Deals with a mouseover at x, z
    private void MouseOverHexagon(int x, int z)
    {
        HexInfo info = ChunkManager.instance.GetHexAt(x, z);
        if (mouseOverX != x || mouseOverZ != z)
        {
            ClearMouseOver();
            info.setMouseover(true);
            mouseOverX = x;
            mouseOverZ = z;

            if (towerCreationID != -1)
            {
                ghostObj.SetActive(true);
                ghostObj.transform.position = info.transform.position;

                if (info.IsOccupied())
                    ghostObj.GetComponent<Renderer>().material.color = unavailColor;
                else
                    ghostObj.GetComponent<Renderer>().material.color = availColor;
            }
        }
    }

    // Checks raycast from mouse to see if a hexagon is there
    private HexInfo CheckRaycast()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = (1 << 3);
        RaycastHit hit;
        if(Physics.Raycast(r, out hit, 1000, mask))
        {
            Transform objHit = hit.transform;
            if(objHit.tag == "Grid")
            {
                HexInfo objHI = objHit.GetComponent<HexInfo>();
                return objHI;
            }
        }
        return null;
    }

    public void UI_RequestTower(int towerID)
    {
        selecting = true;
        towerCreationID = towerID;
    }
}
