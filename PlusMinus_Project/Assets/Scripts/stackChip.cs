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
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateGameObject()
    {
        int slotI = Random.Range(0, 2);

        GameObject myChip = Instantiate(myObject, Slot[slotI].transform.position, Quaternion.identity) as GameObject;

        myChip.transform.SetParent(Slot[slotI].transform);
        
    }

    public void OnClickCheckChip()
    {
        CreateGameObject();
    }
}
