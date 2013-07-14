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
using JetBrains.Annotations;

namespace ImageEvolver.Core.Utilities
{
    /// <summary>
    ///     This class is used to wrap another IDisposable
    ///     It is useful for keeping track of ownership of an object (i.e. if the object should be disposed by owner or not)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class OwnedObject<T> : IDisposable where T : IDisposable
    {
        private readonly bool _owned;

        public OwnedObject(T value, bool owned)
        {
            Value = value;
            _owned = owned;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
                if (IsOwned)
                {
                    Value.Dispose();
                    Value = default(T);
                }
            }
        }

        [PublicAPI]
        public bool IsOwned
        {
            get { return _owned; }
        }

        [PublicAPI]
        public T Value { get; private set; }
    }
}