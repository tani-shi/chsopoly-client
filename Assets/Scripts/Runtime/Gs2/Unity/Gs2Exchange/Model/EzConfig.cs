/*
 * Copyright 2016 Game Server Services, Inc. or its affiliates. All Rights
 * Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */
using Gs2.Gs2Exchange.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;


namespace Gs2.Unity.Gs2Exchange.Model
{
	[Preserve]
	public class EzConfig
	{
		/** 名前 */
		public string Key { get; set; }
		/** 値 */
		public string Value { get; set; }

		public EzConfig()
		{

		}

		public EzConfig(Gs2.Gs2Exchange.Model.Config @config)
		{
			Key = @config.key;
			Value = @config.value;
		}

        public Config ToModel()
        {
            return new Config {
                key = Key,
                value = Value,
            };
        }
	}
}
