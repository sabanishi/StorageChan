using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sabanishi.MainGame.Stage
{
    public class StagePresenter:MonoBehaviour
    {
        [SerializeField] private StageView _view;
        private StageModel _model;
        public StageModel Model => _model;
        
        private Vector3 _playerRespawnPos;
        private Vector2 _mapSize;
        public Vector2 MapSize => _mapSize;

        public void Initialize(Tilemap tilemap)
        {
            var chipData = ConvertToChipEnumArray(tilemap);
            _model = new StageModel();
            _mapSize=new Vector2(chipData.GetLength(0),chipData.GetLength(1));
            _view.Initialize(chipData.GetLength(0), chipData.GetLength(1));

            //modelとの紐付け
            _model.StageData.ObserveAdd().Subscribe(_view.OnStageChipAdded).AddTo(gameObject);
            _model.StageData.ObserveReplace().Subscribe(_view.OnStageChipReplaced).AddTo(gameObject);
            _model.StageData.ObserveRemove().Subscribe(_view.OnStageChipRemoved).AddTo(gameObject);
            
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
        private ChipData[,] ConvertToChipEnumArray(Tilemap tilemap)
        {
            tilemap.CompressBounds();
            var bounds = tilemap.cellBounds;
            ChipData[,] chipData = new ChipData[bounds.size.x, bounds.size.y];
            TileBase[] allBlocks = tilemap.GetTilesBlock(bounds);
            for(int x=0;x<bounds.size.x;x++)
            {
                for(int y=0;y<bounds.size.y;y++)
                {
                    var tile = allBlocks[x + y * bounds.size.x];
                    var chipEnum = ChipEnum.None;
                    if(tile is not null)
                    {
                        if (tile.name.Contains("Floor"))
                        {
                            chipEnum = ChipEnum.CanPaintBlock;
                        }else if (tile.name.Contains("Floor2"))
                        {
                            chipEnum= ChipEnum.CannotPaintBlock;   
                        }else if (tile.name.Equals("Start"))
                        {
                            _playerRespawnPos = new Vector3(x, y, 0);
                        }else if (tile.name.Equals("Box"))
                        {
                            chipEnum = ChipEnum.Box;
                        }
                    }
                    
                    if (!chipEnum.Equals(ChipEnum.None))
                    {
                        //tileの画像を取得する
                        var sprite = ((Tile)tile)?.sprite;
                        chipData[x, y] = new ChipData(chipEnum, sprite,x,y);
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