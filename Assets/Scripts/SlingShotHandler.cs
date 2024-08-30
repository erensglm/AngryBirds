using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{
    [Header ("Line Renderers")]

    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    [Header("Transform Reference")]

    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;


    [Header("Slingshot Stats")]
    [SerializeField] private float _maxDistance =  3.5f;
    [SerializeField] private float _shotForce = 5f;
    [SerializeField] private float _timeBetweenBirdRespawns = 2f;
    [SerializeField] private float _elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve _elasticCurve;

    [Header("Scripts")]

    [SerializeField] private SlingShotArea _slingShotArea;


    [Header("Bird")]
    [SerializeField] private AngieBird _angieBirdPrefab;
    [SerializeField] private float _angieBirdPositionOffset = 2f;


    private Vector2 _slingShotLinePositions;
    private Vector2 _direction;
    private Vector2 _directionNormalized;

    private bool _clickedWithinArea;
    private bool _birdOnSlingshot;

    private AngieBird _spawnedAngieBird;

    private void Awake()
    {
        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnAngieBird();
    }

    private void Update()
    {

        if (InputManager.WasLeftMouseButtonPressed && _slingShotArea.isWithinSlingshotArea())
        {
           _clickedWithinArea = true;
        }
        
        if (InputManager.IsLeftMousePressed && _clickedWithinArea && _birdOnSlingshot)
        {
            DrawSlingShot();
            PositionAndRotateAngieBird();
        }

        if(InputManager.WasLeftMouseButtonReleased && _birdOnSlingshot && _clickedWithinArea)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                _clickedWithinArea = false;
                _birdOnSlingshot = false;

                _spawnedAngieBird.LaunchBird(_direction, _shotForce);
                GameManager.instance.UseShot();
                AnimateSlinshot();


                if (GameManager.instance.HasEnoughShots())
                { 
                    StartCoroutine(SpawnAngieBirdAfterTime());
                }
            }

        }

    }


    #region SlingShot Methods


    private void DrawSlingShot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        _slingShotLinePositions = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position , _maxDistance);

        SetLines(_slingShotLinePositions);

        _direction = (Vector2)_centerPosition.position - _slingShotLinePositions;
        _directionNormalized = _direction.normalized;
    }

    private void SetLines(Vector2 position)
    {

        if (!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);

        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);
    }


    private IEnumerator SpawnAngieBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBetweenBirdRespawns);
        SpawnAngieBird();
    }

    #endregion


    #region Angie Bird Methods

    public void SpawnAngieBird()
    {
        SetLines(_idlePosition.position);

        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _angieBirdPositionOffset;

        _spawnedAngieBird = Instantiate(_angieBirdPrefab, spawnPosition, Quaternion.identity);
        _spawnedAngieBird.transform.right = dir;

        _birdOnSlingshot = true;
    }

  

    private void PositionAndRotateAngieBird()
    {
        // Position the bird at the center of the slingshot

        _spawnedAngieBird.transform.position = _slingShotLinePositions + _directionNormalized * _angieBirdPositionOffset;  
        _spawnedAngieBird.transform.right = _directionNormalized;  
    }

    #endregion


    #region Animate Slingshot

    private void AnimateSlinshot()
    {
       _elasticTransform.position = _leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);

        float time = dist / _elasticDivider;

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransform, time));

        
    }

    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            SetLines(trans.position);

            yield return null;
        }
    }

    #endregion
}
