using System;
using MainGame.Stage;
using UniRx;
using UnityEngine;

namespace Sabanishi.MainGame.Stage
{
    public class StageModel
    {
        private readonly ReactiveCollection<ChipData> _stageData;
        public IReadOnlyReactiveCollection<ChipData> StageData => _stageData;

        private Subject<ChipData> _dropBoxSubject;
        public ISubject<ChipData> DropBoxSubject => _dropBoxSubject;

        public Func<int, int, GameObject> GetChipObject;

        public StageModel()
        {
            _stageData = new ();
            _dropBoxSubject = new ();
        }

        public void Dispose()
        {
            _stageData.Dispose();
            _dropBoxSubject.Dispose();
        }

        /// <summary>
        /// chipDataを基にステージを作成する
        /// </summary>
        public void CreateBlock(ChipData[,] chipData)
        {
            for (int y = 0; y < chipData.GetLength(1); y++)
            {
                for(int x = 0; x < chipData.GetLength(0); x++)
                {
                    if (chipData[x, y] != null)
                    {
                        _stageData.Add(chipData[x, y]);
                    }
                }
            }
        }

        public void RemoveBox(ChipData chipData)
        {
            _stageData.Remove(chipData);
            DropBox(chipData);
        }
        
        private void DropBox(ChipData chipData)
        {
            //上の箱を落とす
            foreach (var chip in _stageData)
            {
                if (chip.Y == chipData.Y + 1 && chip.X == chipData.X)
                {
                    if (!chip.ChipEnum.Equals(ChipEnum.Box)) break;
                    if (!IsStick(chip.X, chip.Y))
                    {
                        Debug.Log($"DropBox:{chip.X},{chip.Y}");
                        _dropBoxSubject.OnNext(chip);
                        DropBox(chip);
                        chip.SetY(chip.Y-1);
                        break;
                    }
                }
            }
        }

        private bool IsStick(int x,int y)
        {
            //上
            if (IsStickSub(x, y - 1, Direction.Down)) return true;
            if (IsStickSub(x + 1, y, Direction.Right)) return true;
            if (IsStickSub(x - 1, y, Direction.Left)) return true;
            return false;
        }

        private bool IsStickSub(int x, int y, Direction dir)
        {
            var obj = GetChipObject.Invoke(x, y);
            if (obj != null)
            {
                var chip = obj.GetComponent<BlockChip>();
                if (chip != null && chip.CanStick(dir))
                {
                    return true;
                }
            }
            return false;
        }

        public void AddBox(ChipData chipData)
        {
            _stageData.Add(chipData);
        }

        /// <summary>
        /// 全ての箱が室内に存在すればtrueを返す
        /// </summary>
        public bool IsAllBlockIndoor(int x)
        {
            foreach (var chipData in _stageData)
            {
                if (chipData.ChipEnum.Equals(ChipEnum.Box) && chipData.X < x)
                {
                    return false;
                }
            }
            return true;
        }
    }
}