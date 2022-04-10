using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexInfo : MonoBehaviour
{
    private Color selectedColor = new Color(0.25f, 1f, 0.25f, 1f);
    private Color mouseoverColor = new Color(0, 1, 1);
    public int x, z;
    private MeshRenderer myRenderer;
    private GameObject selectedVisual;
    private GameObject mouseoverVisual;
    private BaseTower tower = null;

    private void Awake()
    {
        myRenderer = GetComponent<MeshRenderer>();
        selectedVisual = transform.GetChild(0).gameObject;
        mouseoverVisual = transform.GetChild(1).gameObject;


        selectedVisual.GetComponent<Renderer>().material.color = selectedColor;
        mouseoverVisual.GetComponent<Renderer>().material.color = mouseoverColor;
    }

    public void ClearTower()
    {
        tower = null;
    }

    public bool SetTower(BaseTower t)
    {
        if (!IsOccupied())
        {
            tower = t;
            return true;
        }
        return false;
    }

    // deals damage to tower if has one
    public void TakeDamage(int dmg)
    {
        if(tower != null)
        {
            bool dead = tower.TakeDamage(dmg);
            if(dead)
            {
                Coordinate mC = new Coordinate(x, z);
                Coordinate cP = mC.GetChunkPos();
                ChunkManager.instance.GetChunkAt(cP).DestroyTower(mC);
            }
        }
    }

    public BaseTower GetTower()
    {
        return tower;
    }

    public bool IsOccupied()
    {
        return tower != null;
    }    

    public void setSelected(int type)
    {
        selectedVisual.SetActive(false);
        if(type == 1)
        {
            selectedVisual.SetActive(true);
        }
    }

    public void setMouseover(bool mouse)
    {
        mouseoverVisual.SetActive(mouse);
    }
}
