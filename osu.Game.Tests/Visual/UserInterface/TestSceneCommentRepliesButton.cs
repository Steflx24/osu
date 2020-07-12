﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Overlays.Comments.Buttons;
using osu.Framework.Graphics;
using osu.Framework.Allocation;
using osu.Game.Overlays;
using osu.Framework.Graphics.Containers;
using osuTK;
using NUnit.Framework;
using System.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Testing;

namespace osu.Game.Tests.Visual.UserInterface
{
    public class TestSceneCommentRepliesButton : OsuTestScene
    {
        [Cached]
        private readonly OverlayColourProvider colourProvider = new OverlayColourProvider(OverlayColourScheme.Blue);

        private readonly TestButton button;

        public TestSceneCommentRepliesButton()
        {
            Child = new FillFlowContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 10),
                Children = new Drawable[]
                {
                    button = new TestButton(),
                    new LoadRepliesButton(),
                    new ShowRepliesButton(1),
                    new ShowRepliesButton(2)
                }
            };
        }

        [Test]
        public void TestArrowRotation()
        {
            AddStep("Toggle icon up", () => button.ToggleIcon(true));
            AddAssert("Icon facing upwards", () => button.Icon.Scale.Y == -1);
            AddStep("Toggle icon down", () => button.ToggleIcon(false));
            AddAssert("Icon facing downwards", () => button.Icon.Scale.Y == 1);
        }

        private class TestButton : CommentRepliesButton
        {
            public SpriteIcon Icon => this.ChildrenOfType<SpriteIcon>().First();

            public TestButton()
            {
                Text = "sample text";
            }

            public new void ToggleIcon(bool upwards) => base.ToggleIcon(upwards);
        }
    }
}
