using UnityEngine;

namespace BeetleBattlePredictor;

internal sealed class PredictorOverlay : MonoBehaviour
{
    private GUIStyle _panelStyle;
    private GUIStyle _headerStyle;
    private GUIStyle _opponentStyle;
    private GUIStyle _counterStyle;
    private GUIStyle _hintStyle;
    private Texture2D _background;
    private Texture2D _accent;
    private float _toggleNoticeUntil;
    private float _nextPollAt;

    public PredictorOverlay(IntPtr pointer) : base(pointer)
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Plugin.IsEnabled.Value = !Plugin.IsEnabled.Value;
            if (!Plugin.IsEnabled.Value)
            {
                PredictionState.Clear();
            }
            _toggleNoticeUntil = Time.unscaledTime + 2.5f;
            Plugin.ModLog.LogInfo($"Prediction overlay: {(Plugin.IsEnabled.Value ? "ON" : "OFF")}");
        }

        if (Plugin.IsEnabled.Value && Time.unscaledTime >= _nextPollAt)
        {
            _nextPollAt = Time.unscaledTime + 0.1f;
            EnemyMoveReader.Poll();
        }
    }

    private void OnGUI()
    {
        var showToggleNotice = Time.unscaledTime < _toggleNoticeUntil;
        if (!showToggleNotice && !Plugin.IsEnabled.Value)
        {
            return;
        }

        EnsureStyles();
        if (showToggleNotice && (!Plugin.IsEnabled.Value || !PredictionState.HasPrediction))
        {
            var notice = Plugin.IsEnabled.Value ? "甲壳虫预测：已开启" : "甲壳虫预测：已关闭";
            var noticeRect = new Rect(Plugin.PositionX.Value, Plugin.PositionY.Value, 300f, 58f);
            DrawPanel(noticeRect);
            GUI.Label(new Rect(noticeRect.x + 20f, noticeRect.y + 8f, 266f, 42f), notice, _opponentStyle);
            return;
        }

        if (!PredictionState.HasPrediction)
        {
            var waitingRect = new Rect(Plugin.PositionX.Value, Plugin.PositionY.Value, 390f, 72f);
            DrawPanel(waitingRect);
            GUI.Label(new Rect(waitingRect.x + 20f, waitingRect.y + 6f, 350f, 26f), "甲壳虫出招预测", _headerStyle);
            GUI.Label(new Rect(waitingRect.x + 20f, waitingRect.y + 31f, 350f, 36f),
                "正在锁定对手招式…", _opponentStyle);
            return;
        }

        var text = Prediction.Describe(PredictionState.OpponentMove);
        var width = Math.Min(430f, Screen.width - Plugin.PositionX.Value - 20f);
        var rect = new Rect(Plugin.PositionX.Value, Plugin.PositionY.Value, width, 150f);

        DrawPanel(rect);
        GUI.Label(new Rect(rect.x + 20f, rect.y + 6f, rect.width - 38f, 28f),
            $"甲壳虫出招预测   第 {PredictionState.Turn + 1} 回合", _headerStyle);
        GUI.Label(new Rect(rect.x + 20f, rect.y + 34f, rect.width - 38f, 40f),
            $"对手   {text.Opponent}", _opponentStyle);
        GUI.Label(new Rect(rect.x + 20f, rect.y + 72f, rect.width - 38f, 42f),
            $"应对   {text.Counter}", _counterStyle);
        GUI.Label(new Rect(rect.x + 20f, rect.y + 116f, rect.width - 38f, 27f),
            "结果已锁定  ·  F8 关闭", _hintStyle);
    }

    private void DrawPanel(Rect rect)
    {
        GUI.Box(rect, GUIContent.none, _panelStyle);
        GUI.DrawTexture(new Rect(rect.x, rect.y + 8f, 5f, rect.height - 16f), _accent);
    }

    private void EnsureStyles()
    {
        var size = Math.Clamp(Plugin.FontSize.Value, 16, 42);
        if (_panelStyle is not null && _opponentStyle!.fontSize == size)
        {
            return;
        }

        _background = new Texture2D(1, 1);
        _background.SetPixel(0, 0, new Color(0.025f, 0.04f, 0.055f, 0.96f));
        _background.Apply();
        _accent = new Texture2D(1, 1);
        _accent.SetPixel(0, 0, new Color(0.15f, 0.82f, 0.92f, 1f));
        _accent.Apply();

        _panelStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = _background },
            border = new RectOffset(8, 8, 8, 8),
        };
        _headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Math.Max(15, size - 10),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = new Color(0.45f, 0.9f, 0.96f) },
        };
        _opponentStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = size,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = new Color(1f, 0.82f, 0.25f) },
        };
        _counterStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Math.Min(42, size + 2),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = new Color(0.42f, 1f, 0.55f) },
        };
        _hintStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Math.Max(13, size - 12),
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = new Color(0.7f, 0.78f, 0.82f) },
        };
    }
}
