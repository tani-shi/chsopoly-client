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
using Gs2.Gs2Matchmaking.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;


namespace Gs2.Unity.Gs2Matchmaking.Model
{
	[Preserve]
	public class EzAttributeRange
	{
		/** 属性名 */
		public string Name { get; set; }
		/** ギャザリング参加可能な属性値の最小値 */
		public int Min { get; set; }
		/** ギャザリング参加可能な属性値の最大値 */
		public int Max { get; set; }

		public EzAttributeRange()
		{

		}

		public EzAttributeRange(Gs2.Gs2Matchmaking.Model.AttributeRange @attributeRange)
		{
			Name = @attributeRange.name;
			Min = @attributeRange.min.HasValue ? @attributeRange.min.Value : 0;
			Max = @attributeRange.max.HasValue ? @attributeRange.max.Value : 0;
		}

        public AttributeRange ToModel()
        {
            return new AttributeRange {
                name = Name,
                min = Min,
                max = Max,
            };
        }
	}
}
