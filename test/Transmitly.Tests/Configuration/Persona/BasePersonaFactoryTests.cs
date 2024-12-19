﻿// ﻿﻿Copyright (c) Code Impressions, LLC. All Rights Reserved.
//  
//  Licensed under the Apache License, Version 2.0 (the "License")
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using AutoFixture;
using Transmitly.Tests;

namespace Transmitly.Persona.Configuration.Tests
{
    [TestClass()]
    public class BasePersonaFactoryTests : BaseUnitTest
    {
        [TestMethod()]
        public async Task GetAllShouldReturnRegistrations()
        {
            var registrations = fixture.Create<IEnumerable<IPersonaRegistration>>();
            fixture.Inject(registrations);
            var sut = fixture.Create<BasePersonaFactory>();
            var results = await sut.GetAllAsync();

            Assert.AreEqual(registrations.Count(), results.Count);
            CollectionAssert.AreEquivalent(registrations.ToList(), results.ToList());
        }

        [TestMethod()]
        public async Task GetShouldReturnRegistration()
        {
            var registrations = fixture.Create<IEnumerable<IPersonaRegistration>>();
            fixture.Inject(registrations);
            var sut = fixture.Create<BasePersonaFactory>();
            var result = await sut.GetAsync(registrations.First().Name);
            Assert.IsNotNull(result);
            Assert.AreEqual(registrations.First().Name, result.Name);

            result = await sut.GetAsync(fixture.Create<string>());
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AnyMatchShouldEnforceRequiredParameters()
        {
            var registrations = fixture.Create<IEnumerable<IPersonaRegistration>>();
            fixture.Inject(registrations);
            var sut = fixture.Create<BasePersonaFactory>();
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => sut.AnyMatch<UnitTestPersona>(null!, []));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => sut.AnyMatch<UnitTestPersona>(fixture.Create<string>(), null!));
        }

        class UnitTestPersona
        {

        }
    }
}