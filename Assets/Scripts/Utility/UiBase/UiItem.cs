using UnityEngine;
using UnityEngine.UI;

namespace ASeKi.Ui
{
    public class UiItem : MonoBehaviour
    {
        public int Index { get; protected set; }

        [SerializeField] protected GameObject[] goes = null;
        [SerializeField] protected Button[] buttons = null;
        [SerializeField] protected Image[] images = null;
        [SerializeField] protected Text[] texts = null;

        public Button GetButton(int i)
        {
            return buttons[i];
        }

        public Image GetImage(int i)
        {
            return images[i];
        }

        public Text GetText(int i)
        {
            return texts[i];
        }

        public GameObject GetGo(int i)
        {
            return goes[i];
        }
    }

    public abstract class UiItem<T> : MonoBehaviour
    {
        public int Index { get; protected set; }
        public T Data { get; protected set; }

        [SerializeField] protected GameObject[] goes = null;
        [SerializeField] protected Button[] buttons = null;
        [SerializeField] protected Image[] images = null;
        [SerializeField] protected Text[] texts = null;

        public Button GetButton(int i)
        {
            return buttons[i];
        }

        public Image GetImage(int i)
        {
            return images[i];
        }

        public Text GetText(int i)
        {
            return texts[i];
        }

        public GameObject GetGo(int i)
        {
            return goes[i];
        }

        public abstract void UpdateItem(int index, T data);
    }
}