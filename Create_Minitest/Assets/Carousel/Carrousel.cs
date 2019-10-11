using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public class Carrousel : MonoBehaviour
{
    enum ScrollDirection { Up, Down, None};

    public int _NumberOfImages = 10;
    public int _Radius = 20;
    public float _SpriteOrienataion = 0.0f;
    

    private bool _isMoving = false;
    private ScrollDirection _scrollDir = ScrollDirection.None;
    public float _ScrollDelayTime = 0.75f;
    private float _scrollTimer = 0.0f;
    private Vector3 _RotationStart = new Vector3(0, 0, 0);
    private Vector3 _RotationTarget = new Vector3(0, 0, 0);


    public float _TransitionSpeed = 100.0f;
    public float _ScrollSpeed = 200.0f;
    private float _StartTime = 0.0f;
    private float _AngleDistance = 0.0f;

    private float _AngleOffset = 0.0f;

    public bool _Paused = true;

    // Start is called before the first frame update
    void Start()
    {
        BuildImages();
    }

    // Update is called once per frame
    void Update()
    {
        
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

   

    public void BuildImages()
    {
        _AngleOffset = 360.0f / _NumberOfImages;

        transform.localPosition = new Vector3(transform.position.x, transform.position.y, _Radius);

        //while (transform.childCount > 0)
        //{
        //    SafeDestroy(transform.GetChild(0).gameObject);
        //}

        //GameObject[] sprites = new GameObject[_NumberOfImages];

        //for (int i = 0; i < _NumberOfImages; i++)
        //{
        //    var newImg = (GameObject)Instantiate(Resources.Load("Image"));
        //    newImg.name = i.ToString();
        //    newImg.transform.parent = transform;
        //    newImg.GetComponent<ImageOrientation>().Orienataion = _SpriteOrienataion;
        //    newImg.GetComponent<ImageOrientation>().UpdateOrientation();
        //    sprites[i] = newImg;
        //    var angle = _AngleOffset * (Mathf.PI / 180);
        //    float x = _Radius * Mathf.Sin(i * angle);
        //    float y = 0;
        //    float z = _Radius * Mathf.Cos(i * angle);
        //    sprites[i].transform.localPosition = new Vector3(x,y,-z);
        //}
    }

    public static T SafeDestroy<T>(T obj) where T : Object
    {

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
        if (_isMoving != false) return;
        if (isRight)
        {
            _RotationTarget.y = _RotationStart.y + _AngleOffset;
        }
        else
        {
            _RotationTarget.y = _RotationStart.y - _AngleOffset;
        }

        _StartTime = Time.time;
        _AngleDistance = Mathf.Abs(Mathf.Abs(_RotationTarget.y) - Mathf.Abs(_RotationStart.y));
        _isMoving = true;

    }

    
    public bool LerpToNext()
    {
        // Distance moved equals elapsed time times speed..
        float distCovered = (Time.time - _StartTime) * _TransitionSpeed;

        // Fraction of journey completed equals current distance divided by total distance.
        float currAng = transform.localEulerAngles.y;
        float frctDist = distCovered / _AngleDistance;
        float currDist = Mathf.Abs(Mathf.Abs(_RotationTarget.y) - Mathf.Abs((_RotationTarget.y > 0.0f ? currAng : currAng - 360)));

        // Set our position as a fraction of the distance between the markers.
        var toMoveLerp = Vector3.Lerp(_RotationStart, _RotationTarget, frctDist);

        transform.localEulerAngles = toMoveLerp;
        var isArrived = (currDist <= 0.1f && currDist < _AngleDistance);
        
        if (currDist >= 360)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
            isArrived = true;
        }

        if (!isArrived) return false;

        _RotationStart.y = transform.localEulerAngles.y;

        return true;
        
    }

    private void MouseScrolling()
    {
        if (_Paused)
            return;
        var scrollInput = Input.mouseScrollDelta.y;
        if (Math.Abs(scrollInput) > 0.01f)
        {
            _scrollTimer = 0.0f;
            if (scrollInput > 0.0f)
            {
                _RotationStart.y += Time.deltaTime * _ScrollSpeed;
                transform.localEulerAngles = _RotationStart;
                _scrollDir = ScrollDirection.Up;
            }
            else
            {
                _RotationStart.y -= Time.deltaTime * _ScrollSpeed;
                transform.localEulerAngles = _RotationStart;
                _scrollDir = ScrollDirection.Down;
            }
        }
        else
        {
            if (_scrollDir == ScrollDirection.None || _isMoving) return;
            _scrollTimer += Time.deltaTime;
            if (!(_scrollTimer >= _ScrollDelayTime)) return;
            LerpToClosest();
            _scrollTimer = 0.0f;
            _scrollDir = ScrollDirection.None;
        }
    }

    public bool LerpToClosest()
    {
        var i = Mathf.RoundToInt(_RotationStart.y / _AngleOffset);
        
        _RotationTarget.y = i * _AngleOffset;

        float currDist = Mathf.Abs(Mathf.Abs(_RotationTarget.y) - Mathf.Abs(_RotationStart.y));

        _StartTime = Time.time;
        _AngleDistance = Mathf.Abs(Mathf.Abs(_RotationTarget.y) - Mathf.Abs(_RotationStart.y));
        _isMoving = true;

        return false;
    }

    public int FowardIndex()
    {
        if (Mathf.RoundToInt(transform.eulerAngles.y) == 0 && Mathf.RoundToInt(_AngleOffset) == 0) return 0;
        return (Mathf.RoundToInt(transform.eulerAngles.y) / Mathf.RoundToInt(_AngleOffset)) % _NumberOfImages;
    }
}

