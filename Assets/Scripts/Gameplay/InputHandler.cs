using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    private Color32 availColor = new Color32(52, 255, 59, 161);
    private Color32 unavailColor = new Color32(255, 0, 0, 161);

    public static InputHandler instance;
    public float minCamY, maxCamY;
    private float scrollSpeed = 3;

    private int selectedX = int.MaxValue, selectedZ = int.MaxValue;
    private int mouseOverX = int.MaxValue, mouseOverZ = int.MaxValue;
    private bool selecting = false;
    private BaseTower towerCreation = null;
    private BaseTower towerSelected = null;

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
        {
            int index = TowerManager.instance.CreateTower(1);
            BaseTower t = TowerManager.instance.GetUnplacedTower(index);
            UIManager.instance.CreateCard(t);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
            UIManager.instance.SelectCard(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            UIManager.instance.SelectCard(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            UIManager.instance.SelectCard(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            UIManager.instance.SelectCard(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            UIManager.instance.SelectCard(4);

        if (Input.GetKeyDown(KeyCode.T) && WaveManager.instance.building)
            UIManager.instance.ToggleFusionUI();
        if (Input.GetKeyDown(KeyCode.Escape))
            UIManager.instance.HideFusionUI();

        // if we are in build phase, selecting and we press E, pick up tower
        // if we have a card out and press F, add to fusion
        if (WaveManager.instance.building)
        {
            if (towerSelected != null && towerSelected.IsRetrievable())
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    int index = TowerManager.instance.PickupTower(new Coordinate(selectedX, selectedZ));
                    if (index != -1)
                    {
                        UIManager.instance.CreateCard(towerSelected, true);
                        ClearAll();
                    }
                }
            }
            if (towerCreation != null && towerCreation.HasUpgrade())
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    UIManager.instance.AddFusionCard(towerCreation);
                    UIManager.instance.ShowFusionUI();
                    ClearAll();
                }
            }
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            float diff = Input.mouseScrollDelta.y * scrollSpeed;
            float cyPos = Camera.main.transform.localPosition.y;
            Vector3 newPos = Camera.main.transform.localPosition + new Vector3(0, -diff, 0);
            newPos = new Vector3(newPos.x, Mathf.Clamp(newPos.y, minCamY, maxCamY), newPos.z);
            Camera.main.transform.localPosition = newPos;
        }

        if (Input.GetMouseButtonDown(1))
            ClearAll();

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

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            HexInfo obj = CheckRaycast();
            if (selecting)
            {
                // if placing a tower
                if (obj != null)
                {
                    int x = obj.x;
                    int z = obj.z;
                    MouseOverHexagon(x, z);
                    if (Input.GetMouseButtonDown(0))
                        SelectHexagon(x, z);
                }
                else
                {
                    ClearMouseOver();
                }
            }
            else
            {
                // check for if object moused over is a tower, so we can show info if necessary
                if (obj != null)
                {
                    if (obj.IsOccupied())
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
    }

    public void ClearAll()
    {
        towerCreation = null;
        selecting = false;
        ghostObj.SetActive(false);
        ClearMouseOver();
        ClearSelected();

        UIManager.instance.UnselectCard();
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
        towerSelected = null;
        UIManager.instance.DestroyWorldCard();
        UIManager.instance.HideRangeIndicator();
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
        UIManager.instance.HideFusionUI();
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
            if (towerCreation != null)
            {
                if (!ChunkManager.instance.IsHexOccupied(x, z))
                {
                    bool success = TowerManager.instance.PlaceTower(new Coordinate(x, z), towerCreation);
                    if (success)
                    {
                        UIManager.instance.DestroySelectedCard();
                        soundFX.inst.playTowerPlacement();
                    }
                }

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
            towerSelected = info.GetTower();
            UIManager.instance.CreateWorldCard(towerSelected);
            UIManager.instance.ShowRangeIndicator(towerSelected.transform.position, towerSelected.GetRange());
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
            
            if (towerCreation != null)
            {
                ghostObj.SetActive(true);
                ghostObj.transform.position = info.transform.position;
                UIManager.instance.ShowRangeIndicator(info.transform.position, towerCreation.GetRange());

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

    public void UI_RequestTower(BaseTower tower)
    {
        selecting = true;
        towerCreation = tower;
    }
}
