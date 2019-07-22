using UnityEngine;

public class ProjectIcons : ProjectReferences<ProjectIcons>
{
    [SerializeField]
    private Texture2D linkBlack;
    public static Texture2D LinkBlack => Instance.linkBlack;

    [SerializeField]
    private Texture2D linkWhite;
    public static Texture2D LinkWhite => Instance.linkWhite;

    [SerializeField]
    private Texture2D linkBlue;
    public static Texture2D LinkBlue => Instance.linkBlue;
}
