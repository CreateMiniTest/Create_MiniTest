using System;
using UnityEngine;
using Object = UnityEngine.Object;


public class Carrousel : MonoBehaviour
{
    enum ScrollDirection { Up, Down, None};

    public int NumberOfImages;
    public int Radius;
    public float SpriteOrienataion;

    private bool _isMoving;
    private ScrollDirection _scrollDir = ScrollDirection.None;
    private Vector3 _rotationStart = new Vector3(0, 0, 0);
    private Vector3 _rotationTarget = new Vector3(0, 0, 0);


    public float TransitionTime;
    public float ScrollSpeed;
    public float ScrollDelayTime;

    private float _scrollTimer = 0.0f;
    private float _startTime = 0.0f;
    private float _angleDistance = 0.0f;
    private float _angleOffset = 0.0f;

    public bool Paused = true;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    private void OnGUI()
    {
        if (_isMoving)
        {
            _isMoving = !LerpToNext();   
        }
        else
        {
            MouseScrolling();
        }
    }

    public static Vector3 PosAroundCircle(float angle, float radius)
    {
        float x = radius * Mathf.Sin(angle);
        float y = 0;                  
        float z = radius * Mathf.Cos(angle);
        return new Vector3(x, y, -z);
    }

    public void SetUp()
    {
        _angleOffset = 360.0f / NumberOfImages;

        transform.localPosition = new Vector3(transform.position.x, transform.position.y, Radius);

        int count = 0;
        while (count < 20)
        {
            transform.GetChild(count).localPosition = new Vector3(-200,-200,-200);
            transform.GetChild(count).gameObject.SetActive(false);
            count++;
        }

        for (int i = 0; i < NumberOfImages; i++)
        {
            var img = transform.GetChild(i).gameObject;
            img.SetActive(true);
            img.name = i.ToString();
            img.transform.localPosition = new Vector3(0, 0, 0);

            //Points around the center of the carousel
            var angle = _angleOffset * (Mathf.PI / 180);
            img.transform.localPosition = PosAroundCircle(i * angle, Radius);

            img.GetComponent<ImageOrientation>().Orienataion = SpriteOrienataion;
            img.GetComponent<ImageOrientation>().UpdateOrientation();
        }

        LerpToClosest();
    }

    public void BuildImages()
    {
        if (!Application.isEditor) return;

        _angleOffset = 360.0f / NumberOfImages;

        transform.localPosition = new Vector3(transform.position.x, transform.position.y, Radius);

        while (transform.childCount > 0)
        {
            SafeDestroy(transform.GetChild(0).gameObject);
        }

        GameObject[] sprites = new GameObject[NumberOfImages];

        for (int i = 0; i < NumberOfImages; i++)
        {
            sprites[i] = (GameObject)Instantiate(Resources.Load("Image"));
            sprites[i].name = i.ToString();
            sprites[i].transform.parent = transform;
            sprites[i].GetComponent<ImageOrientation>().Orienataion = SpriteOrienataion;
            sprites[i].GetComponent<ImageOrientation>().UpdateOrientation();
            var angle = _angleOffset * (Mathf.PI / 180);
            sprites[i].transform.localPosition = PosAroundCircle(i * angle, Radius);
        }
    }

    public static T SafeDestroy<T>(T obj) where T : Object
    {
        if(Application.isEditor)
            DestroyImmediate(obj);
        else
            Destroy(obj);

        return null;
    }
    public static T SafeDestroyGameObject<T>(T component) where T : Component
    {
        if (component != null)
            SafeDestroy(component.gameObject);
        return null;
    }


    public void MoveCarouselToNext(bool isRight = true)
    {
        if (_isMoving) return;
        if (isRight)
        {
            _rotationTarget.y = _rotationStart.y + _angleOffset;
        }
        else
        {
            _rotationTarget.y = _rotationStart.y - _angleOffset;
        }

        _startTime = Time.time;
        _angleDistance = Mathf.Abs(Mathf.Abs(_rotationTarget.y) - Mathf.Abs(_rotationStart.y));
        _isMoving = true;

    }

    
    public bool LerpToNext()
    {
        float fraction = (Time.time - _startTime) / TransitionTime;

        float currAng = transform.localEulerAngles.y;
        float currDist = Mathf.Abs(Mathf.Abs(_rotationTarget.y) - Mathf.Abs((_rotationTarget.y > 0.0f ? currAng : currAng - 360)));

        var toMoveLerp = new Vector3(0, Mathf.SmoothStep(_rotationStart.y, _rotationTarget.y, fraction), 0);

        transform.localEulerAngles = toMoveLerp;
        var isArrived = (currDist <= 0.1f && currDist < _angleDistance);
        
        if (currDist >= 360)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
            isArrived = true;
        }

        if (!isArrived) return false;

        _rotationStart.y = transform.localEulerAngles.y;

        return true;
    }

    private void MouseScrolling()
    {
        if (Paused)
            return;
        var scrollInput = Input.mouseScrollDelta.y;
        if (Math.Abs(scrollInput) > 0.01f)
        {
            _scrollTimer = 0.0f;
            if (scrollInput > 0.0f)
            {
                _rotationStart.y += Time.deltaTime * ScrollSpeed;
                transform.localEulerAngles = _rotationStart;
                _scrollDir = ScrollDirection.Up;
            }
            else
            {
                _rotationStart.y -= Time.deltaTime * ScrollSpeed;
                transform.localEulerAngles = _rotationStart;
                _scrollDir = ScrollDirection.Down;
            }
        }
        else
        {
            if (_scrollDir == ScrollDirection.None || _isMoving) return;
            _scrollTimer += Time.deltaTime;
            if (!(_scrollTimer >= ScrollDelayTime)) return;
            LerpToClosest();
            _scrollTimer = 0.0f;
            _scrollDir = ScrollDirection.None;
        }
    }

    public bool LerpToClosest()
    {
        var i = Mathf.RoundToInt(_rotationStart.y / _angleOffset);
        
        _rotationTarget.y = i * _angleOffset;
        _startTime = Time.time;
        _angleDistance = Mathf.Abs(Mathf.Abs(_rotationTarget.y) - Mathf.Abs(_rotationStart.y));
        _isMoving = true;

        return false;
    }

    public int FowardIndex()
    {
        if (Mathf.RoundToInt(transform.eulerAngles.y) == 0 && Mathf.RoundToInt(_angleOffset) == 0) return 0;
        return (Mathf.RoundToInt(transform.eulerAngles.y) / Mathf.RoundToInt(_angleOffset)) % NumberOfImages;
    }
}

