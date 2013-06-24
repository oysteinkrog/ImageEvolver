#region Copyright

//     ImageEvolver
//     Copyright (C) 2013-2013 Øystein Krog
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Affero General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Affero General Public License for more details.
// 
//     You should have received a copy of the GNU Affero General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;

namespace ImageEvolver.Core.Settings
{
    public class SettingMinMidInt : SettingMinMid<int>
    {
        private readonly ISettingsParent _parent;
        private int _mid;
        private int _min;

        public SettingMinMidInt(ISettingsParent parent, int min, int mid)
        {
            _parent = parent;
            _min = min;
            _mid = mid;
        }

        public override int Mid
        {
            get { return _mid; }
            protected internal set { _mid = Math.Max(value, Min); }
        }

        public override int Min
        {
            get { return _min; }
            protected internal set { _min = Math.Min(value, Mid); }
        }
    }
}