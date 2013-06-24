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
using ImageEvolver.Core.Utilities;

namespace ImageEvolver.Core.Settings
{
    public class SettingMinMaxValInt : SettingMinMaxVal<int>
    {
        private int _max;
        private int _min;
        private int _value;

        public SettingMinMaxValInt(int value)
        {
            _value = value;
        }

        public override int Max
        {
            get { return _max; }
            protected internal set { _max = Math.Max(value, Min); }
        }

        public override int Min
        {
            get { return _min; }
            protected internal set { _min = Math.Min(value, Max); }
        }

        public override int Value
        {
            get { return _value; }
            protected internal set { _value = MathUtils.Clamp(value, Min, Max); }
        }
    }
}