using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 5f;      
    public float moveLimitY = 4f;     

    void Update()
    {
        float moveDir = 0f;


         if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveDir = 1f;
            else if (Keyboard.current.sKey.isPressed) moveDir = -1f;
        }

        

        Vector3 newPos = transform.position + Vector3.up * moveDir * moveSpeed * Time.deltaTime;


        newPos.y = Mathf.Clamp(newPos.y, -moveLimitY, moveLimitY);


        transform.position = newPos;
    }
}
