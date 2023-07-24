using System;
using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame
{
    public class PlayerPresenter:MonoBehaviour
    {
        [SerializeField] private PlayerView _view;
        [SerializeField]private PlayerSurroundingCollider _surroundingCollider;
        private PlayerModel _model;
        private Transform _transform;
        
        private Subject<ChipData> _addBoxSubject;
        public IObservable<ChipData> AddBoxSubject => _addBoxSubject;
        private Subject<ChipData> _removeBoxSubject;
        public IObservable<ChipData> RemoveBoxSubject => _removeBoxSubject;
        
        public PlayerModel Model => _model;

        /// <summary>
        /// 初期化関数
        /// </summary>
        public void Initialize(Vector3 startPos)
        {
            _addBoxSubject = new();
            _removeBoxSubject= new();
            
            _transform = transform;
            _model = new PlayerModel();
            _view.Initialize();
            _model.Initialize(startPos,GetComponent<BoxCollider2D>());
            _surroundingCollider.Initialize();

            _model.Pos.Subscribe(_view.OnPosChanged).AddTo(gameObject);
            _model.IsAir.Subscribe(_view.OnIsAirChanged).AddTo(gameObject);
            _model.IsHang.Subscribe(_view.OnIsHangChanged).AddTo(gameObject);
            _model.IsPaintMode.Subscribe(_view.OnIsPaintModeChanged).AddTo(gameObject);
            _model.NowBodyDirection.Subscribe(_view.OnBodyDirectionChanged).AddTo(gameObject);
            _model.OnUpdateSpeedSubject.Subscribe(_view.OnSpeedChanged).AddTo(gameObject);
            _model.PutBoxSubject.Subscribe(PutBox).AddTo(gameObject);
            _model.RemoveBoxSubject.Subscribe(RemoveBox).AddTo(gameObject);
            _model.CheckCanPaintAction = _view.CheckCanPaint;
            _model.CheckIsAirAction = _view.CheckIsAir;
            _model.PlayPaintAction = _view.PlayPaintAnimation;
            _model.CheckBoxAction = _view.CheckBox;
            _model.CheckCanPutBox = _view.CheckCanPutBox;

            _view.PosChangeSubject.Subscribe(_model.OnPosChanged).AddTo(gameObject);

            _surroundingCollider.OnAdd.Subscribe(_model.AddNearChipCollider).AddTo(gameObject);
            _surroundingCollider.OnRemove.Subscribe(_model.RemoveNearChipCollider).AddTo(gameObject);
        }

        /// <summary>
        /// 破壊時処理
        /// </summary>
        public void Dispose()
        {
            _addBoxSubject.Dispose();
            _removeBoxSubject.Dispose();
            
            _model.Dispose();
            _view.Dispose();
            _surroundingCollider.Dispose();
        }

        public void Update()
        {
            bool isAir = _view.CheckIsAir(_model.NowBodyDirection.Value);
            _model.Update(_transform.position,isAir);
        }

        public void PutBox(ChipData chipData)
        {
            _addBoxSubject.OnNext(chipData);
        }

        public void RemoveBox(ChipData chipData)
        {
            _removeBoxSubject.OnNext(chipData);
        }
    }
}