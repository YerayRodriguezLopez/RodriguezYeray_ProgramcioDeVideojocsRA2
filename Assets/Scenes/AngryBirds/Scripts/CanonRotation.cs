using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonRotation : MonoBehaviour, InputActions.IShootActions
{
    public Vector3 _maxRotation;
    public Vector3 _minRotation;
    private float _offset = -51.6f;
    public GameObject ShootPoint;
    public GameObject Bullet;
    public float ProjectileSpeed = 0;
    public float MaxSpeed;
    public float MinSpeed;
    public GameObject PotencyBar;
    private float _initialScaleX;
    private Vector2 _distanceBetweenMouseAndPlayer;
    private bool isRaising = false;
    [SerializeField] private float _multiplier = 10f;
    private InputActions _inputActions;
    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new InputActions();
            _inputActions.Shoot.SetCallbacks(this);
        }
        _inputActions.Shoot.Enable();
    }
    private void OnDisable()
    {
        _inputActions.Shoot.Disable();
    }
    private void Awake()
    {
        _initialScaleX = PotencyBar.transform.localScale.x;
    }
    void Update()
    {
         //obtenir el valor del click del cursor (Fer amb new input system)
         Vector2 mousePosition = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        //obtenir el vector distància entre el canó i el cursor
        _distanceBetweenMouseAndPlayer = mousePosition - (Vector2)transform.position;
        var ang = (Mathf.Atan2(_distanceBetweenMouseAndPlayer.y, _distanceBetweenMouseAndPlayer.x) * 180f / Mathf.PI);
        //en quin dels tres eixos va l'angle?
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(ang + _offset, _minRotation.z, _maxRotation.z));

        if (isRaising)
        {
            //acotar entre dos valors (mirar variables)
            ProjectileSpeed = Mathf.Clamp(ProjectileSpeed + _multiplier * Time.deltaTime, MinSpeed, MaxSpeed);
            CalculateBarScale();
        }
        
        CalculateBarScale();

    }
    public void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, ProjectileSpeed / MaxSpeed),
            transform.localScale.y,
            transform.localScale.z);
    }
    //detectar cuando el jugador apreta el boton y ir llenando la barra hasta que suelte el boton
    public void OnShoot(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRaising = true;
        }
        if (context.canceled)
        {
            isRaising = false;
            Shoot();
            ProjectileSpeed = 0;
            CalculateBarScale();
        }
    }
    public void Shoot()
    {
        GameObject bullet = Instantiate(Bullet, ShootPoint.transform.position, ShootPoint.transform.rotation);
        Vector2 direction = CalculateBulletDirection();
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * ProjectileSpeed;
    }
    public Vector2 CalculateBulletDirection()
    {
        Vector2 direction = _distanceBetweenMouseAndPlayer.normalized;
        if (Vector3.Angle(Vector2.right, direction) > _maxRotation.z)
        {
            float angleInRad = _maxRotation.z * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad));
        }
        else if (Vector3.Angle(Vector2.right, direction) < _minRotation.z)
        {
            float angleInRad = _minRotation.z * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad));
        }
        return direction;
    }
}
