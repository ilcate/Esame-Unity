using System.Threading.Tasks;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject camera1;
    public GameObject camera2;
    private Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public async Task ActivateGameCam()
    {
        animator.SetTrigger("change");

        await Task.Delay(1000);

        camera2.SetActive(true);
        camera1.SetActive(false);
    }
}
