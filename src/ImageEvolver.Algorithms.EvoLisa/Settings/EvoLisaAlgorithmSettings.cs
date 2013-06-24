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
using ImageEvolver.Core;
using ImageEvolver.Core.Settings;

namespace ImageEvolver.Algorithms.EvoLisa.Settings
{
    public class EvoLisaAlgorithmSettings : IAlgorithmSettings, ISettingsParent
    {
        public EvoLisaAlgorithmSettings()
        {
            AddPointMutationRate = new SettingValInt(this, 1500);
            AddPolygonMutationRate = new SettingValInt(this, 700);

            AlphaMutationRate = new SettingValInt(this, 1500);
            AlphaRange = new SettingMinMaxInt(this, 30, 60);

            BlueMutationRate = new SettingValInt(this, 1500);
            BlueRange = new SettingMinMaxInt(this, 0, 255);

            GreenMutationRate = new SettingValInt(this, 1500);
            GreenRange = new SettingMinMaxInt(this, 0, 255);

            MovePointMutationRate = new SettingMinMidMaxInt(this, 1500, 1500, 1500);

            MovePointRange = new SettingMinMidInt(this, 3, 20);
            MovePolygonMutationRate = new SettingValInt(this, 700);
            PointsRange = new SettingMinMaxInt(this, 0, 1500);

            PointsPerPolygonRange = new SettingMinMaxInt(this, 3, 10);
            PolygonsRange = new SettingMinMaxInt(this, 0, 255);

            RedMutationRate = new SettingValInt(this, 1500);
            RedRange = new SettingMinMaxInt(this, 0, 255);

            RemovePointMutationRate = new SettingValInt(this, 1500);
            RemovePolygonMutationRate = new SettingValInt(this, 1500);
        }

        public SettingValInt AddPointMutationRate { get; set; }

        public SettingValInt AddPolygonMutationRate { get; set; }

        public SettingValInt AlphaMutationRate { get; set; }

        public SettingMinMaxInt AlphaRange { get; set; }

        public SettingValInt BlueMutationRate { get; set; }

        public SettingMinMaxInt BlueRange { get; set; }

        public SettingValInt GreenMutationRate { get; set; }

        public SettingMinMaxInt GreenRange { get; set; }

        public SettingMinMidMaxInt MovePointMutationRate { get; set; }

        public SettingMinMidInt MovePointRange { get; set; }

        public SettingValInt MovePolygonMutationRate { get; set; }

        public SettingMinMaxInt PointsPerPolygonRange { get; set; }
        public SettingMinMaxInt PointsRange { get; set; }

        public SettingMinMaxInt PolygonsRange { get; set; }

        public SettingValInt RedMutationRate { get; set; }

        public SettingMinMaxInt RedRange { get; set; }

        public SettingValInt RemovePointMutationRate { get; set; }

        public SettingValInt RemovePolygonMutationRate { get; set; }

        public void CopyAllFrom(IAlgorithmSettings modified)
        {
            throw new NotImplementedException();
        }
    }
}