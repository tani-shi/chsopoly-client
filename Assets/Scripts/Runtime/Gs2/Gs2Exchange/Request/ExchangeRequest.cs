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
using System;
using System.Collections.Generic;
using System.Linq;
using Gs2.Core.Control;
using Gs2.Core.Model;
using Gs2.Gs2Exchange.Model;
using LitJson;
using UnityEngine.Scripting;

namespace Gs2.Gs2Exchange.Request
{
	[Preserve]
	public class ExchangeRequest : Gs2Request<ExchangeRequest>
	{

        /** ネームスペース名 */
        public string namespaceName { set; get; }

        /**
         * ネームスペース名を設定
         *
         * @param namespaceName ネームスペース名
         * @return this
         */
        public ExchangeRequest WithNamespaceName(string namespaceName) {
            this.namespaceName = namespaceName;
            return this;
        }


        /** 交換レートの種類名 */
        public string rateName { set; get; }

        /**
         * 交換レートの種類名を設定
         *
         * @param rateName 交換レートの種類名
         * @return this
         */
        public ExchangeRequest WithRateName(string rateName) {
            this.rateName = rateName;
            return this;
        }


        /** 交換するロット数 */
        public int? count { set; get; }

        /**
         * 交換するロット数を設定
         *
         * @param count 交換するロット数
         * @return this
         */
        public ExchangeRequest WithCount(int? count) {
            this.count = count;
            return this;
        }


        /** 設定値 */
        public List<Config> config { set; get; }

        /**
         * 設定値を設定
         *
         * @param config 設定値
         * @return this
         */
        public ExchangeRequest WithConfig(List<Config> config) {
            this.config = config;
            return this;
        }


        /** 重複実行回避機能に使用するID */
        public string duplicationAvoider { set; get; }

        /**
         * 重複実行回避機能に使用するIDを設定
         *
         * @param duplicationAvoider 重複実行回避機能に使用するID
         * @return this
         */
        public ExchangeRequest WithDuplicationAvoider(string duplicationAvoider) {
            this.duplicationAvoider = duplicationAvoider;
            return this;
        }


        /** アクセストークン */
        public string accessToken { set; get; }

        /**
         * アクセストークンを設定
         *
         * @param accessToken アクセストークン
         * @return this
         */
        public ExchangeRequest WithAccessToken(string accessToken) {
            this.accessToken = accessToken;
            return this;
        }

    	[Preserve]
        public static ExchangeRequest FromDict(JsonData data)
        {
            return new ExchangeRequest {
                namespaceName = data.Keys.Contains("namespaceName") && data["namespaceName"] != null ? data["namespaceName"].ToString(): null,
                rateName = data.Keys.Contains("rateName") && data["rateName"] != null ? data["rateName"].ToString(): null,
                count = data.Keys.Contains("count") && data["count"] != null ? (int?)int.Parse(data["count"].ToString()) : null,
                config = data.Keys.Contains("config") && data["config"] != null ? data["config"].Cast<JsonData>().Select(value =>
                    {
                        return Config.FromDict(value);
                    }
                ).ToList() : null,
                duplicationAvoider = data.Keys.Contains("duplicationAvoider") && data["duplicationAvoider"] != null ? data["duplicationAvoider"].ToString(): null,
            };
        }

	}
}