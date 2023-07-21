using Cinemachine;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class MainCamera:MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField]private CinemachineConfiner _cameraConfiner;
        [SerializeField] private PolygonCollider2D _limitColliderPrefab;
        [SerializeField] private float _cameraDepth;
        [SerializeField] private BoxCollider2D _borderCollider;

        public void Initialize(Transform playerTransform,Vector2 screenSize)
        {
            //カメラ移動区域の設定
            PolygonCollider2D limit = _limitColliderPrefab;
            Vector2[] path = new Vector2[4];
            limit.pathCount = 1;
            path[0] = new Vector2(-0.5f, -0.5f);
            path[1] = new Vector2(-0.5f, screenSize.y-0.5f);
            path[2] = new Vector2(screenSize.x-0.5f, screenSize.y-0.5f);
            path[3] = new Vector2(screenSize.x - 0.5f, -0.5f);
            limit.SetPath(0, path);
            
            _cameraConfiner.m_BoundingShape2D = limit;
            _camera.m_Lens.OrthographicSize = _cameraDepth;
            _camera.Follow = playerTransform;
            
            //画面端のcolliderの設定
            _borderCollider.size = new Vector2(1, screenSize.y+5f);
            _borderCollider.gameObject.transform.position = new Vector2(-1, screenSize.y / 2f - 0.5f);
        }
    }
}