using UnityEngine;
using System.Collections;

public class MeasureLine : MonoBehaviour
{
    public Vector3 m_p1;
    public Vector3 m_p2;

    public void SetPoint(Vector3 p1, Vector3 p2)
    {
        m_p1 = p1;
        m_p2 = p2;
    }
    // Use this for initialization
    void Start()
    {
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 y = m_p2 - m_p1;
        var scale = y.magnitude / 2;

        y.Normalize();

        Vector3 x = Vector3.Cross(y, Vector3.up);
        x.Normalize();

        Vector3 z = Vector3.Cross(x, y);
        z.Normalize();

        Matrix4x4 rotMat = new Matrix4x4(x, y, z, new Vector4(0, 0, 0, 1));

        Matrix4x4 translate = Matrix4x4.Translate(new Vector3(0, scale, 0));
        translate =  rotMat * translate;

        transform.localRotation = rotMat.rotation;
        transform.localPosition = new Vector3(translate.m03, translate.m13, translate.m23) + m_p1;
        transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
    }
}
