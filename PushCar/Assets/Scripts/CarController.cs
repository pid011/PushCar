using System.Net;
using System.Net.Sockets;
using System.Text;

using UnityEngine;

public class CarController : MonoBehaviour
{
    private Vector2 _startPos;
    private AudioSource _moveSound;
    private float _speed;
    private GameObject _flag;

    private int _networkFlag;

    private void Awake()
    {
        _moveSound = GetComponent<AudioSource>();
        _flag = GameObject.Find("flag");
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    _speed = 0.3f;
        //}

        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 초기 클릭 좌표를 저장
            _startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 마우스에서 손을 땠을 때의 좌표를 구함
            Vector2 endPos = Input.mousePosition;
            float swipeLength = endPos.x - _startPos.x;

            // 두 위치의 차이를 속도로 활용
            _speed = swipeLength / 500f;

            _moveSound.Play();

            _networkFlag = 1;
        }

        transform.Translate(_speed, 0, 0);
        _speed *= 0.98f;

        // 속도를 체크해서 거의 멈췄을때 네트워크에 연결
        if (_speed < 0.0001f && _networkFlag == 1)
        {
            _networkFlag = 0;
            float length = _flag.transform.position.x - transform.position.x;
            if (length < 0) length = -99.99f;

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            byte[] buffer = Encoding.UTF8.GetBytes(length.ToString());

            EndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 10200);

            clientSocket.SendTo(buffer, serverEP);

            var recvBytes = new byte[1024];
            int nrecv = clientSocket.ReceiveFrom(recvBytes, ref serverEP);
            string txt = Encoding.UTF8.GetString(recvBytes, 0, nrecv);

            Debug.Log(txt);
        }
    }
}
