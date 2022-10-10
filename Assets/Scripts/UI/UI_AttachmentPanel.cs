using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AttachmentPanel : MonoBehaviour
{
    public string[] customizationAttachments;
    public GameObject[] gameObjectAttachments;
    public GameObject customizationGroupPrefab;

    public Transform customizationGroupContainer;
    public Transform Pos;

    private Transform weapon;
    private Transform muzzle;
    private GameObject attachment;

    private bool wasAttached = false;

    void Start() {

        ArrayList attachmentList = new ArrayList();

        try {
            weapon = Pos.GetChild(0);
            muzzle = weapon.Find("muzzle");
        } catch {
            print("cannot find muzzle, please confirm muzzle is a child of weapon prefab");
        }

        foreach (GameObject attachmentPrefab in gameObjectAttachments) {
            foreach (string part in customizationAttachments) {
                if (attachmentPrefab.name == part) {
                    GameObject customizationGroup = Instantiate(customizationGroupPrefab);
                    customizationGroup.transform.SetParent(customizationGroupContainer, false);

                    Transform label = customizationGroup.transform.Find("Label"); 
                    Transform attach = customizationGroup.transform.Find("Attach");
                    label.GetComponent<TMPro.TextMeshProUGUI>().text = part;

                    attach.GetComponent<Button>().onClick.AddListener(() => {
                        if (wasAttached == false) {
                            attachment = Instantiate(attachmentPrefab);
                            attachment.transform.position = muzzle.position;
                            wasAttached = true;
                            attach.GetComponent<TMPro.TextMeshProUGUI>().text = "Detach";
                        }
                        else {
                            Destroy(attachment);
                            wasAttached = false;
                            attach.GetComponent<TMPro.TextMeshProUGUI>().text = "Attach";
                        }
                    });
                }
            }
            attachmentList.Add(attachmentPrefab);
        }
    }
}
