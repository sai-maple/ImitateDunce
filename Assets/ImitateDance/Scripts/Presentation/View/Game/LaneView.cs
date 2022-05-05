using System.Collections.Generic;
using System.Linq;
using ImitateDance.Scripts.Applications.Data;
using ImitateDance.Scripts.Applications.Enums;
using UnityEngine;

namespace ImitateDance.Scripts.Presentation.View.Game
{
    public sealed class LaneView : MonoBehaviour
    {
        [SerializeField] private List<NoteView> _noteViews = default;
        [SerializeField] private RectTransform _content = default;
        [SerializeField] private RectTransform _currentLine = default;

        private Vector2 _position = default;

        // 楽譜データを渡して表示する
        public void Initialize(ScoreData scoreData)
        {
            for (var i = 0; i < _noteViews.Count; i++)
            {
                _noteViews[i].Initialize(scoreData.Score.Any(note => note.Beat == i));
            }
        }

        // ターン交代中に、変化したNoteを隠す
        public void HideAll()
        {
            foreach (var note in _noteViews)
            {
                note.Hide();
            }
        }

        // ターンの音楽再生中、再生位位置に線を移動させる
        public void Play(float normalize)
        {
            _position.x = _content.sizeDelta.x * normalize;
            _currentLine.anchoredPosition = _position;
        }

        // 入力を受けて、タイミングに応じたNoteを変化させる
        public void Dance(int beet, DanceDirection direction, bool isSuccess)
        {
            _noteViews[beet].Judge(direction, isSuccess);
        }
    }
}