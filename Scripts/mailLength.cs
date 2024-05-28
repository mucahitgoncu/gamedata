using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class mailLength : MonoBehaviour
{
    public Image mailImage;
    public TextMeshProUGUI mailText;

    private RectTransform imageRectTransform;
    public void ChangeLength()
    {
        AdjustImageHeight();
    }

    private void AdjustImageHeight()
    {
        imageRectTransform = mailImage.GetComponent<RectTransform>();
        // Ensure mailText is properly updated before getting preferredHeight
        mailText.ForceMeshUpdate();

        // Adjust the height of the mailImage to match the preferred height of the mailText
        float preferredHeight = mailText.preferredHeight;

        imageRectTransform.sizeDelta = new Vector2(imageRectTransform.sizeDelta.x, preferredHeight + 160);
    }
}
