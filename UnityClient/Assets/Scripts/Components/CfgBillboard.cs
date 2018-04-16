using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CfgBillboard : MonoBehaviour {
	// Update is called once per frame
    public enum BillboardType
    {
        enNormal,
        enVertical,
    }
    public BillboardType m_type;
	void Update () {
        if (m_type == BillboardType.enNormal)
        {
            Transform cameraTrans = Camera.main.transform;
            Vector3 target = this.transform.position + this.transform.position - cameraTrans.position;
            this.transform.LookAt(target, cameraTrans.rotation * Vector3.up);
        }
        else
        {
            Transform cameraTrans = Camera.main.transform;
            Vector3 v = cameraTrans.transform.position - this.transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(cameraTrans.transform.position - v);  
        }
	}
}
