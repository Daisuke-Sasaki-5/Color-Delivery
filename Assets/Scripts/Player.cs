using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    /// <summary>
    /// プレイヤー操作全般を管理
    /// </summary>

    //変数宣言
    [Header("移動速度")]
    [SerializeField] private float moveSpeed = 3.0f;

    [Header("取得距離")]
    [SerializeField] private float pickupRadius = 2f;

    [Header("手持ち表示用モデル")]
    [SerializeField] private GameObject[] handItmes;

    [SerializeField] private LayerMask piuckupLayer;

    private ItemType? currentItem = null;
    public ItemType? CurrentItem => currentItem;

    // 入力管理
    private PlayerInputActions inputActions;
    private bool interacPressed;

    private Vector2 moveInput;
    private Rigidbody rb;

    /// <summary>
    /// 初期化
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Interact.performed -= OnInteract;

        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);

        if(moveDir.magnitude > 1f)
        {
            moveDir = moveDir.normalized;
        }

        Vector3 velocity = rb.linearVelocity;

        velocity.x = moveDir.x * moveSpeed;
        velocity.z = moveDir.z * moveSpeed;

        rb.linearVelocity = velocity;

        if(moveDir != Vector3.zero)
        {
            transform.forward = moveDir;
        }
    }

    // インタラクト処理
    private void OnInteract(InputAction.CallbackContext context)
    { 
        Debug.Log("Eキー");

        // 既に持っている場合returnする
        if (currentItem != null)
        {
            Debug.Log("既に持っている");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius,piuckupLayer);

        foreach(Collider hit in hits)
        {
           PickupObject pickup = hit.GetComponent<PickupObject>();

            if(pickup != null)
            {
                currentItem = pickup.itemType;
                ShowHand(currentItem.Value);

                Debug.Log(currentItem + "取得");

                pickup.gameObject.SetActive(false);

                break;
            }
        }
    }

    private void ShowHand(ItemType itemType)
    {
        foreach(var item in handItmes)
        {
            item.SetActive(false);
        }

        handItmes[(int)itemType].SetActive(true);
    }

    // アイテムをクリアする
    public void ClearItem()
    {
        currentItem = null;

        foreach(var item in handItmes)
        {
            item.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            pickupRadius);
    }
}

