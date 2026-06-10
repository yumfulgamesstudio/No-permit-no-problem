using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    public GameObject icon;
    public float speed;
    public float size = 1f;

    public void Update()
    {
        float y = Mathf.PingPong(Time.time * speed, size);
        icon.transform.localPosition = new Vector3(0, y, 0);
    }
}
