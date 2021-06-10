using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    [SerializeField] private GameObject _flag;
    [SerializeField] private Text _record;

    private Rigidbody2D _rigidbody;
    private Vector2 _startPos;
    private AudioSource _moveSound;
    private float _speed;

    private int _networkFlag;

    private void Awake()
    {
        _moveSound = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 초기 클릭 좌표를 저장
            _startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 마우스에서 손을 땠을 때의 좌표를 구함
            Vector2 endPos = Input.mousePosition;
            var swipeLength = endPos.x - _startPos.x;
            _rigidbody.AddForce(Vector2.right * (swipeLength * 0.1f), ForceMode2D.Impulse);

            _moveSound.Play();
            _networkFlag = 1;
        }

        if (_rigidbody.velocity.x > 0.001f || _networkFlag != 1) return;

        // 속도를 체크해서 거의 멈췄을때 네트워크에 연결
        _networkFlag = 0;
        var length = _flag.transform.position.x - transform.position.x;
        if (length < 0) length = -99.99f;

        var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        var id = "pid011";
        var buffer = Encoding.UTF8.GetBytes($"{id};;{length}");

        EndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 10200);

        clientSocket.SendTo(buffer, serverEP);

        var recvBytes = new byte[1024];
        var nrecv = clientSocket.ReceiveFrom(recvBytes, ref serverEP);
        var txt = Encoding.UTF8.GetString(recvBytes, 0, nrecv);

        _record.text = txt;
    }
}
