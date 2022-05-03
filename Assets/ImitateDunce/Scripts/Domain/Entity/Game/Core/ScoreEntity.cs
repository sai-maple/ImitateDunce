using System;
using System.Collections.Generic;
using ImitateDunce.Applications.Data;
using ImitateDunce.Applications.Enums;
using UniRx;
using UnityEngine;

namespace ImitateDunce.Domain.Entity.Game.Core
{
    public sealed class ScoreEntity : IDisposable
    {
        private readonly Subject<DunceData> _subject = default;
        private readonly Dictionary<int, DunceDirection> _demo = default;
        private readonly Dictionary<int, DunceDirection> _dunce = default;
        private ScoreDto _score = default;

        public ScoreEntity()
        {
            _subject = new Subject<DunceData>();
            _demo = new Dictionary<int, DunceDirection>();
            _dunce = new Dictionary<int, DunceDirection>();
        }

        // タップしたタイミングのNoteを返す
        public IObservable<DunceData> OnDunceAsObservable()
        {
            return _subject.Share();
        }

        // demo dunceの中身を空の譜面にする
        public void SetScore(ScoreDto score)
        {
            _score = score;
            _demo.Clear();
            _dunce.Clear();
        }

        // 閾値以内のnoteに入力した方向をセットする
        public void OnDemo(float time, DunceDirection demo)
        {
            foreach (var note in _score.Score)
            {
                if (Mathf.Abs(note.Time - time) > 0.1f) continue;
                if (_demo.ContainsKey(note.Beat)) continue;
                _demo.Add(note.Beat, demo);
                _subject.OnNext(new DunceData(note.Beat, demo));
                break;
            }
        }

        // 閾値以内のnoteに入力した方向をセットして得点を返す
        public int OnDunce(float time, DunceDirection dunceDirection)
        {
            var point = 0;
            foreach (var note in _score.Score)
            {
                // todo threshold and point
                if (Mathf.Abs(note.Time - time) > 0.1f) continue;
                if (_dunce.ContainsKey(note.Beat)) continue;
                _dunce.Add(note.Beat, dunceDirection);
                var demo = _demo.ContainsKey(note.Beat) ? _demo[note.Beat] : DunceDirection.Non;
                _subject.OnNext(new DunceData(note.Beat, demo, dunceDirection));
                break;
            }

            return point;
        }

        public bool IsPerfect()
        {
            foreach (var note in _score.Score)
            {
                var demo = _demo.ContainsKey(note.Beat) ? _demo[note.Beat] : DunceDirection.Non;
                if (!_dunce.ContainsKey(note.Beat)) return false;
                if (demo != _dunce[note.Beat]) return false;
            }

            return true;
        }

        public void Dispose()
        {
            _subject?.OnCompleted();
            _subject?.Dispose();
        }
    }
}