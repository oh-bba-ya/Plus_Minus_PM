using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stackChip : MonoBehaviour
{
    //GameObject chip1;
    public List<GameObject> Slot = new List<GameObject>();
    public GameObject myObject;
    public Button Check;
    int maxChips = 10;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateGameObject()
    {
        if(maxChips > 0)
        {
            int slotI = Random.Range(0, 2);

            GameObject myChip = Instantiate(myObject, Slot[slotI].transform.position, Quaternion.identity) as GameObject;

            myChip.transform.SetParent(Slot[slotI].transform);
            
            maxChips--;
        }        
    }

    public void OnClickCheckChip()
    {
        CreateGameObject();
    }
}
