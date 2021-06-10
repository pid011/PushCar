using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    [SerializeField] private CarController _car;
    [SerializeField] private GameObject _flag;
    [SerializeField] private Text _distance;

    private void Update()
    {
        var length = _flag.transform.position.x - _car.transform.position.x;

        _distance.text = length >= 0 ? $"{length:F}m" : "Game Over";
    }
}
