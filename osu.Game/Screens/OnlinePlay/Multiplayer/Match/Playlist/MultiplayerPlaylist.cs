// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Online.Multiplayer;
using osu.Game.Online.Rooms;

namespace osu.Game.Screens.OnlinePlay.Multiplayer.Match.Playlist
{
    /// <summary>
    /// The multiplayer playlist, containing lists to show the items from a <see cref="MultiplayerRoom"/> in both gameplay-order and historical-order.
    /// </summary>
    public class MultiplayerPlaylist : MultiplayerRoomComposite
    {
        public readonly Bindable<MultiplayerPlaylistDisplayMode> DisplayMode = new Bindable<MultiplayerPlaylistDisplayMode>();

        /// <summary>
        /// Invoked when an item requests to be edited.
        /// </summary>
        public Action<PlaylistItem> RequestEdit;

        private MultiplayerQueueList queueList;
        private MultiplayerHistoryList historyList;
        private bool firstPopulation = true;

        [BackgroundDependencyLoader]
        private void load()
        {
            const float tab_control_height = 25;

            InternalChildren = new Drawable[]
            {
                new OsuTabControl<MultiplayerPlaylistDisplayMode>
                {
                    RelativeSizeAxes = Axes.X,
                    Height = tab_control_height,
                    Current = { BindTarget = DisplayMode }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = tab_control_height + 5 },
                    Masking = true,
                    Children = new Drawable[]
                    {
                        queueList = new MultiplayerQueueList
                        {
                            RelativeSizeAxes = Axes.Both,
                            SelectedItem = { BindTarget = SelectedItem },
                            RequestEdit = item => RequestEdit?.Invoke(item)
                        },
                        historyList = new MultiplayerHistoryList
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0,
                            SelectedItem = { BindTarget = SelectedItem }
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            DisplayMode.BindValueChanged(onDisplayModeChanged, true);
        }

        private void onDisplayModeChanged(ValueChangedEvent<MultiplayerPlaylistDisplayMode> mode)
        {
            historyList.FadeTo(mode.NewValue == MultiplayerPlaylistDisplayMode.History ? 1 : 0, 100);
            queueList.FadeTo(mode.NewValue == MultiplayerPlaylistDisplayMode.Queue ? 1 : 0, 100);
        }

        protected override void OnRoomUpdated()
        {
            base.OnRoomUpdated();

            if (Room == null)
            {
                historyList.Items.Clear();
                queueList.Items.Clear();
                firstPopulation = true;
                return;
            }

            if (firstPopulation)
            {
                foreach (var item in Room.Playlist)
                    addItemToLists(item);

                firstPopulation = false;
            }
        }

        protected override void PlaylistItemAdded(MultiplayerPlaylistItem item)
        {
            base.PlaylistItemAdded(item);
            addItemToLists(item);
        }

        protected override void PlaylistItemRemoved(long item)
        {
            base.PlaylistItemRemoved(item);
            removeItemFromLists(item);
        }

        protected override void PlaylistItemChanged(MultiplayerPlaylistItem item)
        {
            base.PlaylistItemChanged(item);

            removeItemFromLists(item.ID);
            addItemToLists(item);
        }

        private void addItemToLists(MultiplayerPlaylistItem item)
        {
            var apiItem = Playlist.Single(i => i.ID == item.ID);

            if (item.Expired)
                historyList.Items.Add(apiItem);
            else
                queueList.Items.Add(apiItem);
        }

        private void removeItemFromLists(long item)
        {
            queueList.Items.RemoveAll(i => i.ID == item);
            historyList.Items.RemoveAll(i => i.ID == item);
        }
    }
}
