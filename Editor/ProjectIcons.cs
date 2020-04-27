using UnityEngine;

namespace Ogxd.ProjectCurator
{
    public static class ProjectIcons
    {
        private static Texture2D linkBlack;
        public static Texture2D LinkBlack => linkBlack ?? (linkBlack = Resources.Load<Texture2D>("link_black"));

        private static Texture2D linkWhite;
        public static Texture2D LinkWhite => linkWhite ?? (linkWhite = Resources.Load<Texture2D>("link_white"));

        private static Texture2D linkBlue;
        public static Texture2D LinkBlue => linkBlue ?? (linkBlue = Resources.Load<Texture2D>("link_blue"));
    }
}