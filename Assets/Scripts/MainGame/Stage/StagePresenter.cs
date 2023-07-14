using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sabanishi.MainGame.Stage
{
    public class StagePresenter:MonoBehaviour
    {
        [SerializeField] private StageView _view;
        private StageModel _model;
        
        private Vector3 _playerRespawnPos;

        public void Initialize(Tilemap tilemap)
        {
            var chipData = ConvertToChipEnumArray(tilemap);
            _model = new StageModel(chipData.GetLength(0),chipData.GetLength(1));
            _view.Initialize(chipData.GetLength(0), chipData.GetLength(1));

            //modelとの紐付け
            _model.StageData.ObserveAdd().Subscribe(_view.OnStageChipAdded).AddTo(gameObject);
            _model.StageData.ObserveReplace().Subscribe(_view.OnStageChipReplaced).AddTo(gameObject);
            
            _model.CreateBlock(chipData);
        }
        
        /// <summary>
        /// プレイヤーがリスポーンすべき位置を返す
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPlayerRespawnPos()
        {
            return _playerRespawnPos;
        }

        /// <summary>
        /// TilemapをChipEnum[,]に変換する
        /// </summary>
        /// <param name="tilemap"></param>
        /// <returns></returns>
        private ChipEnum[,] ConvertToChipEnumArray(Tilemap tilemap)
        {
            tilemap.CompressBounds();
            var bounds = tilemap.cellBounds;
            ChipEnum[,] chipData = new ChipEnum[bounds.size.x, bounds.size.y];
            TileBase[] allBlocks = tilemap.GetTilesBlock(bounds);
            for(int x=0;x<bounds.size.x;x++)
            {
                for(int y=0;y<bounds.size.y;y++)
                {
                    var tile = allBlocks[x + y * bounds.size.x];
                    if(tile is null)
                    {
                        chipData[x, y] = ChipEnum.None;
                    }
                    else
                    {
                        switch (tile.name)
                        {
                            case "Block":
                                chipData[x, y] = ChipEnum.Block;
                                break;
                            case "Start":
                                _playerRespawnPos = new Vector3(x, y, 0);
                                break;
                            default:
                                Debug.LogError("Tile名が不正です");
                                break;
                        }
                    }
                }
            }
            return chipData;
        }

        public void Dispose()
        {
            _model.Dispose();
        }
    }
}