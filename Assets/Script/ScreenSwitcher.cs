using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScreenSwitcher : MonoBehaviour
{
    [System.Serializable]
    public struct TabPair
    {
        public string name;
        public Button button;
        public GameObject panel;
    }

    [Header("ボタンとパネルのペアを登録")]
    public List<TabPair> tabs = new List<TabPair>();

    [Header("全パネルを消すボタン（任意）")]
    public Button hideAllButton;

    void Start()
    {
        // 各タブのボタンにイベントを登録
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i;
            if (tabs[i].button != null)
            {
                tabs[i].button.onClick.AddListener(() => ShowPanel(index));
            }
        }

        // 「全消しボタン」が設定されていればイベントを登録
        if (hideAllButton != null)
        {
            hideAllButton.onClick.AddListener(HideAllPanels);
        }

        // 起動時の状態（最初は0番目を表示する場合）
        // ※起動時に全部消しておきたい場合は、下の3行を消して HideAllPanels(); に書き換えてください
        if (tabs.Count > 0)
        {
            ShowPanel(0);
        }
    }

    // 指定された番号のパネルだけを表示
    public void ShowPanel(int activeIndex)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].panel != null)
            {
                tabs[i].panel.SetActive(i == activeIndex);
            }
        }
    }

    // 登録されているすべてのパネルを非表示にする
    public void HideAllPanels()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            if (tabs[i].panel != null)
            {
                tabs[i].panel.SetActive(false);
            }
        }
    }
}