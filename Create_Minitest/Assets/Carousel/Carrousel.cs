using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public class Carrousel : MonoBehaviour
{
    enum ScrollDirection { Up, Down, None};

    public int _NumberOfImages;
    public int _Radius;
    public float _SpriteOrienataion;

    private bool _isMoving = false;
    private ScrollDirection _scrollDir = ScrollDirection.None;
    private Vector3 _RotationStart = new Vector3(0, 0, 0);
    private Vector3 _RotationTarget = new Vector3(0, 0, 0);


    public float _TransitionTime;
    public float _ScrollSpeed;
    public float _ScrollDelayTime;

    private float _scrollTimer = 0.0f;
    private float _StartTime = 0.0f;
    private float _AngleDistance = 0.0f;
    private float _AngleOffset = 0.0f;

    public bool _Paused = true;

    // Start is called before the first frame update
    void Start()
    {
        SetUp();
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

    public static Vector3 PosAroundCircle(float angle, float radius)
    {
        float x = radius * Mathf.Sin(angle);
        float y = 0;                  
        float z = radius * Mathf.Cos(angle);
        return new Vector3(x, y, -z);
    }

    public void SetUp()
    {
        _AngleOffset = 360.0f / _NumberOfImages;

        transform.localPosition = new Vector3(transform.position.x, transform.position.y, _Radius);

        int count = 0;
        while (count < 20)
        {
            transform.GetChild(count).localPosition = new Vector3(-200,-200,-200);
            transform.GetChild(count).gameObject.SetActive(false);
            count++;
        }

        for (int i = 0; i < _NumberOfImages; i++)
        {
            var img = transform.GetChild(i).gameObject;
            img.SetActive(true);
            img.name = i.ToString();
            img.transform.localPosition = new Vector3(0, 0, 0);

            //Points around the center of the carousel
            var angle = _AngleOffset * (Mathf.PI / 180);
            img.transform.localPosition = PosAroundCircle(i * angle, _Radius);

            img.GetComponent<ImageOrientation>().Orienataion = _SpriteOrienataion;
            img.GetComponent<ImageOrientation>().UpdateOrientation();
        }

        LerpToClosest();
    }

    public void BuildImages()
    {
        if (!Application.isEditor) return;

        _AngleOffset = 360.0f / _NumberOfImages;

        transform.localPosition = new Vector3(transform.position.x, transform.position.y, _Radius);

        while (transform.childCount > 0)
        {
            SafeDestroy(transform.GetChild(0).gameObject);
        }

        GameObject[] sprites = new GameObject[_NumberOfImages];

        for (int i = 0; i < _NumberOfImages; i++)
        {
            sprites[i] = (GameObject)Instantiate(Resources.Load("Image"));
            sprites[i].name = i.ToString();
            sprites[i].transform.parent = transform;
            sprites[i].GetComponent<ImageOrientation>().Orienataion = _SpriteOrienataion;
            sprites[i].GetComponent<ImageOrientation>().UpdateOrientation();
            var angle = _AngleOffset * (Mathf.PI / 180);
            sprites[i].transform.localPosition = PosAroundCircle(i * angle, _Radius);
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
        // Calculate the fraction of the total duration that has passed.
        float fraction = (Time.time - _StartTime) / _TransitionTime;

        float currAng = transform.localEulerAngles.y;
        float currDist = Mathf.Abs(Mathf.Abs(_RotationTarget.y) - Mathf.Abs((_RotationTarget.y > 0.0f ? currAng : currAng - 360)));

         var toMoveLerp = new Vector3(0, Mathf.SmoothStep(_RotationStart.y, _RotationTarget.y, fraction), 0);
        //var toMoveLerp = Vector3.Lerp(_RotationStart, _RotationTarget, frctDist);

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

