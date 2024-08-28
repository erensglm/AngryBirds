using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int MaxNumberOfShots = 3;
    [SerializeField] private float _secondsToWaitBeforeDeathCheck = 3f;

    private int _usedNumberOfShots;

    private IconHandler _iconHandler;

    private List<Baddie> _baddies = new List<Baddie>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _iconHandler = GameObject.FindObjectOfType<IconHandler>();

        Baddie[] foundBaddies = FindObjectsOfType<Baddie>();
        for (int i = 0; i < foundBaddies.Length; i++)
        {
            _baddies.Add(foundBaddies[i]);
        }
    }

    public void UseShot()
    {
        _usedNumberOfShots++;
        _iconHandler.UseShot(_usedNumberOfShots);

        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        return _usedNumberOfShots < MaxNumberOfShots;
    }

    public void CheckForLastShot()
    {
        if (_usedNumberOfShots == MaxNumberOfShots)
        {
            // Eðer IconHandler scriptinde 'ShowLastShot' metodu yoksa bu kod çalýþmaz. Yorum satýrýna aldým.
            // _iconHandler.ShowLastShot();

            StartCoroutine(CheckAfterWaitTime());
        }
    }


    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(_secondsToWaitBeforeDeathCheck);

        if (_baddies.Count == 0)
        {
            WinGame();
        }

        else
        {
            LoseGame();
        }

    }

    public void RemoveBaddie(Baddie baddie)
    {
        _baddies.Remove(baddie);
        CheckForAllDeadBaddies();
    }

    private void CheckForAllDeadBaddies()
    {
        if (_baddies.Count == 0)
        {
            WinGame();
        }
    }
    #region Win/Lose

    private void WinGame()
    {
        UnityEngine.Debug.Log("You win!");
    }

    private void LoseGame()
    {
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}
