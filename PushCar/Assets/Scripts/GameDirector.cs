using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{


    private CarController _car;
    private GameObject _flag;
    private Text _distance;

    private void Start()
    {
        _car = GameObject.Find("car").GetComponent<CarController>();
        _flag = GameObject.Find("flag");
        _distance = GameObject.Find("Distance").GetComponent<Text>();
    }

    private void Update()
    {
        float length = _flag.transform.position.x - _car.transform.position.x;

        _distance.text = length >= 0 ? $"{length:F}m" : "Game Over";
    }
}
