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
using Gs2.Core.Model;
using LitJson;
using UnityEngine.Scripting;

namespace Gs2.Gs2Mission.Model
{
	[Preserve]
	public class Namespace
	{

        /** ネームスペース */
        public string namespaceId { set; get; }

        /**
         * ネームスペースを設定
         *
         * @param namespaceId ネームスペース
         * @return this
         */
        public Namespace WithNamespaceId(string namespaceId) {
            this.namespaceId = namespaceId;
            return this;
        }

        /** オーナーID */
        public string ownerId { set; get; }

        /**
         * オーナーIDを設定
         *
         * @param ownerId オーナーID
         * @return this
         */
        public Namespace WithOwnerId(string ownerId) {
            this.ownerId = ownerId;
            return this;
        }

        /** ネームスペース名 */
        public string name { set; get; }

        /**
         * ネームスペース名を設定
         *
         * @param name ネームスペース名
         * @return this
         */
        public Namespace WithName(string name) {
            this.name = name;
            return this;
        }

        /** ネームスペースの説明 */
        public string description { set; get; }

        /**
         * ネームスペースの説明を設定
         *
         * @param description ネームスペースの説明
         * @return this
         */
        public Namespace WithDescription(string description) {
            this.description = description;
            return this;
        }

        /** ミッション達成時 に実行されるスクリプト のGRN */
        public string missionCompleteTriggerScriptId { set; get; }

        /**
         * ミッション達成時 に実行されるスクリプト のGRNを設定
         *
         * @param missionCompleteTriggerScriptId ミッション達成時 に実行されるスクリプト のGRN
         * @return this
         */
        public Namespace WithMissionCompleteTriggerScriptId(string missionCompleteTriggerScriptId) {
            this.missionCompleteTriggerScriptId = missionCompleteTriggerScriptId;
            return this;
        }

        /** ミッション達成完了時 に実行されるスクリプト のGRN */
        public string missionCompleteDoneTriggerScriptId { set; get; }

        /**
         * ミッション達成完了時 に実行されるスクリプト のGRNを設定
         *
         * @param missionCompleteDoneTriggerScriptId ミッション達成完了時 に実行されるスクリプト のGRN
         * @return this
         */
        public Namespace WithMissionCompleteDoneTriggerScriptId(string missionCompleteDoneTriggerScriptId) {
            this.missionCompleteDoneTriggerScriptId = missionCompleteDoneTriggerScriptId;
            return this;
        }

        /** ミッション達成完了時 にジョブが登録されるネームスペース のGRN */
        public string missionCompleteDoneTriggerQueueNamespaceId { set; get; }

        /**
         * ミッション達成完了時 にジョブが登録されるネームスペース のGRNを設定
         *
         * @param missionCompleteDoneTriggerQueueNamespaceId ミッション達成完了時 にジョブが登録されるネームスペース のGRN
         * @return this
         */
        public Namespace WithMissionCompleteDoneTriggerQueueNamespaceId(string missionCompleteDoneTriggerQueueNamespaceId) {
            this.missionCompleteDoneTriggerQueueNamespaceId = missionCompleteDoneTriggerQueueNamespaceId;
            return this;
        }

        /** カウンター上昇時 に実行されるスクリプト のGRN */
        public string counterIncrementTriggerScriptId { set; get; }

        /**
         * カウンター上昇時 に実行されるスクリプト のGRNを設定
         *
         * @param counterIncrementTriggerScriptId カウンター上昇時 に実行されるスクリプト のGRN
         * @return this
         */
        public Namespace WithCounterIncrementTriggerScriptId(string counterIncrementTriggerScriptId) {
            this.counterIncrementTriggerScriptId = counterIncrementTriggerScriptId;
            return this;
        }

        /** カウンター上昇完了時 に実行されるスクリプト のGRN */
        public string counterIncrementDoneTriggerScriptId { set; get; }

        /**
         * カウンター上昇完了時 に実行されるスクリプト のGRNを設定
         *
         * @param counterIncrementDoneTriggerScriptId カウンター上昇完了時 に実行されるスクリプト のGRN
         * @return this
         */
        public Namespace WithCounterIncrementDoneTriggerScriptId(string counterIncrementDoneTriggerScriptId) {
            this.counterIncrementDoneTriggerScriptId = counterIncrementDoneTriggerScriptId;
            return this;
        }

        /** カウンター上昇完了時 にジョブが登録されるネームスペース のGRN */
        public string counterIncrementDoneTriggerQueueNamespaceId { set; get; }

        /**
         * カウンター上昇完了時 にジョブが登録されるネームスペース のGRNを設定
         *
         * @param counterIncrementDoneTriggerQueueNamespaceId カウンター上昇完了時 にジョブが登録されるネームスペース のGRN
         * @return this
         */
        public Namespace WithCounterIncrementDoneTriggerQueueNamespaceId(string counterIncrementDoneTriggerQueueNamespaceId) {
            this.counterIncrementDoneTriggerQueueNamespaceId = counterIncrementDoneTriggerQueueNamespaceId;
            return this;
        }

        /** 報酬受け取り時 に実行されるスクリプト のGRN */
        public string receiveRewardsTriggerScriptId { set; get; }

        /**
         * 報酬受け取り時 に実行されるスクリプト のGRNを設定
         *
         * @param receiveRewardsTriggerScriptId 報酬受け取り時 に実行されるスクリプト のGRN
         * @return this
         */
        public Namespace WithReceiveRewardsTriggerScriptId(string receiveRewardsTriggerScriptId) {
            this.receiveRewardsTriggerScriptId = receiveRewardsTriggerScriptId;
            return this;
        }

        /** 報酬受け取り完了時 に実行されるスクリプト のGRN */
        public string receiveRewardsDoneTriggerScriptId { set; get; }

        /**
         * 報酬受け取り完了時 に実行されるスクリプト のGRNを設定
         *
         * @param receiveRewardsDoneTriggerScriptId 報酬受け取り完了時 に実行されるスクリプト のGRN
         * @return this
         */
        public Namespace WithReceiveRewardsDoneTriggerScriptId(string receiveRewardsDoneTriggerScriptId) {
            this.receiveRewardsDoneTriggerScriptId = receiveRewardsDoneTriggerScriptId;
            return this;
        }

        /** 報酬受け取り完了時 にジョブが登録されるネームスペース のGRN */
        public string receiveRewardsDoneTriggerQueueNamespaceId { set; get; }

        /**
         * 報酬受け取り完了時 にジョブが登録されるネームスペース のGRNを設定
         *
         * @param receiveRewardsDoneTriggerQueueNamespaceId 報酬受け取り完了時 にジョブが登録されるネームスペース のGRN
         * @return this
         */
        public Namespace WithReceiveRewardsDoneTriggerQueueNamespaceId(string receiveRewardsDoneTriggerQueueNamespaceId) {
            this.receiveRewardsDoneTriggerQueueNamespaceId = receiveRewardsDoneTriggerQueueNamespaceId;
            return this;
        }

        /** 報酬付与処理をジョブとして追加するキューネームスペース のGRN */
        public string queueNamespaceId { set; get; }

        /**
         * 報酬付与処理をジョブとして追加するキューネームスペース のGRNを設定
         *
         * @param queueNamespaceId 報酬付与処理をジョブとして追加するキューネームスペース のGRN
         * @return this
         */
        public Namespace WithQueueNamespaceId(string queueNamespaceId) {
            this.queueNamespaceId = queueNamespaceId;
            return this;
        }

        /** 報酬付与処理のスタンプシートで使用する暗号鍵GRN */
        public string keyId { set; get; }

        /**
         * 報酬付与処理のスタンプシートで使用する暗号鍵GRNを設定
         *
         * @param keyId 報酬付与処理のスタンプシートで使用する暗号鍵GRN
         * @return this
         */
        public Namespace WithKeyId(string keyId) {
            this.keyId = keyId;
            return this;
        }

        /** ミッションのタスクを達成したときのプッシュ通知 */
        public NotificationSetting completeNotification { set; get; }

        /**
         * ミッションのタスクを達成したときのプッシュ通知を設定
         *
         * @param completeNotification ミッションのタスクを達成したときのプッシュ通知
         * @return this
         */
        public Namespace WithCompleteNotification(NotificationSetting completeNotification) {
            this.completeNotification = completeNotification;
            return this;
        }

        /** ログの出力設定 */
        public LogSetting logSetting { set; get; }

        /**
         * ログの出力設定を設定
         *
         * @param logSetting ログの出力設定
         * @return this
         */
        public Namespace WithLogSetting(LogSetting logSetting) {
            this.logSetting = logSetting;
            return this;
        }

        /** 作成日時 */
        public long? createdAt { set; get; }

        /**
         * 作成日時を設定
         *
         * @param createdAt 作成日時
         * @return this
         */
        public Namespace WithCreatedAt(long? createdAt) {
            this.createdAt = createdAt;
            return this;
        }

        /** 最終更新日時 */
        public long? updatedAt { set; get; }

        /**
         * 最終更新日時を設定
         *
         * @param updatedAt 最終更新日時
         * @return this
         */
        public Namespace WithUpdatedAt(long? updatedAt) {
            this.updatedAt = updatedAt;
            return this;
        }

        public void WriteJson(JsonWriter writer)
        {
            writer.WriteObjectStart();
            if(this.namespaceId != null)
            {
                writer.WritePropertyName("namespaceId");
                writer.Write(this.namespaceId);
            }
            if(this.ownerId != null)
            {
                writer.WritePropertyName("ownerId");
                writer.Write(this.ownerId);
            }
            if(this.name != null)
            {
                writer.WritePropertyName("name");
                writer.Write(this.name);
            }
            if(this.description != null)
            {
                writer.WritePropertyName("description");
                writer.Write(this.description);
            }
            if(this.missionCompleteTriggerScriptId != null)
            {
                writer.WritePropertyName("missionCompleteTriggerScriptId");
                writer.Write(this.missionCompleteTriggerScriptId);
            }
            if(this.missionCompleteDoneTriggerScriptId != null)
            {
                writer.WritePropertyName("missionCompleteDoneTriggerScriptId");
                writer.Write(this.missionCompleteDoneTriggerScriptId);
            }
            if(this.missionCompleteDoneTriggerQueueNamespaceId != null)
            {
                writer.WritePropertyName("missionCompleteDoneTriggerQueueNamespaceId");
                writer.Write(this.missionCompleteDoneTriggerQueueNamespaceId);
            }
            if(this.counterIncrementTriggerScriptId != null)
            {
                writer.WritePropertyName("counterIncrementTriggerScriptId");
                writer.Write(this.counterIncrementTriggerScriptId);
            }
            if(this.counterIncrementDoneTriggerScriptId != null)
            {
                writer.WritePropertyName("counterIncrementDoneTriggerScriptId");
                writer.Write(this.counterIncrementDoneTriggerScriptId);
            }
            if(this.counterIncrementDoneTriggerQueueNamespaceId != null)
            {
                writer.WritePropertyName("counterIncrementDoneTriggerQueueNamespaceId");
                writer.Write(this.counterIncrementDoneTriggerQueueNamespaceId);
            }
            if(this.receiveRewardsTriggerScriptId != null)
            {
                writer.WritePropertyName("receiveRewardsTriggerScriptId");
                writer.Write(this.receiveRewardsTriggerScriptId);
            }
            if(this.receiveRewardsDoneTriggerScriptId != null)
            {
                writer.WritePropertyName("receiveRewardsDoneTriggerScriptId");
                writer.Write(this.receiveRewardsDoneTriggerScriptId);
            }
            if(this.receiveRewardsDoneTriggerQueueNamespaceId != null)
            {
                writer.WritePropertyName("receiveRewardsDoneTriggerQueueNamespaceId");
                writer.Write(this.receiveRewardsDoneTriggerQueueNamespaceId);
            }
            if(this.queueNamespaceId != null)
            {
                writer.WritePropertyName("queueNamespaceId");
                writer.Write(this.queueNamespaceId);
            }
            if(this.keyId != null)
            {
                writer.WritePropertyName("keyId");
                writer.Write(this.keyId);
            }
            if(this.completeNotification != null)
            {
                writer.WritePropertyName("completeNotification");
                this.completeNotification.WriteJson(writer);
            }
            if(this.logSetting != null)
            {
                writer.WritePropertyName("logSetting");
                this.logSetting.WriteJson(writer);
            }
            if(this.createdAt.HasValue)
            {
                writer.WritePropertyName("createdAt");
                writer.Write(this.createdAt.Value);
            }
            if(this.updatedAt.HasValue)
            {
                writer.WritePropertyName("updatedAt");
                writer.Write(this.updatedAt.Value);
            }
            writer.WriteObjectEnd();
        }

    	[Preserve]
        public static Namespace FromDict(JsonData data)
        {
            return new Namespace()
                .WithNamespaceId(data.Keys.Contains("namespaceId") && data["namespaceId"] != null ? data["namespaceId"].ToString() : null)
                .WithOwnerId(data.Keys.Contains("ownerId") && data["ownerId"] != null ? data["ownerId"].ToString() : null)
                .WithName(data.Keys.Contains("name") && data["name"] != null ? data["name"].ToString() : null)
                .WithDescription(data.Keys.Contains("description") && data["description"] != null ? data["description"].ToString() : null)
                .WithMissionCompleteTriggerScriptId(data.Keys.Contains("missionCompleteTriggerScriptId") && data["missionCompleteTriggerScriptId"] != null ? data["missionCompleteTriggerScriptId"].ToString() : null)
                .WithMissionCompleteDoneTriggerScriptId(data.Keys.Contains("missionCompleteDoneTriggerScriptId") && data["missionCompleteDoneTriggerScriptId"] != null ? data["missionCompleteDoneTriggerScriptId"].ToString() : null)
                .WithMissionCompleteDoneTriggerQueueNamespaceId(data.Keys.Contains("missionCompleteDoneTriggerQueueNamespaceId") && data["missionCompleteDoneTriggerQueueNamespaceId"] != null ? data["missionCompleteDoneTriggerQueueNamespaceId"].ToString() : null)
                .WithCounterIncrementTriggerScriptId(data.Keys.Contains("counterIncrementTriggerScriptId") && data["counterIncrementTriggerScriptId"] != null ? data["counterIncrementTriggerScriptId"].ToString() : null)
                .WithCounterIncrementDoneTriggerScriptId(data.Keys.Contains("counterIncrementDoneTriggerScriptId") && data["counterIncrementDoneTriggerScriptId"] != null ? data["counterIncrementDoneTriggerScriptId"].ToString() : null)
                .WithCounterIncrementDoneTriggerQueueNamespaceId(data.Keys.Contains("counterIncrementDoneTriggerQueueNamespaceId") && data["counterIncrementDoneTriggerQueueNamespaceId"] != null ? data["counterIncrementDoneTriggerQueueNamespaceId"].ToString() : null)
                .WithReceiveRewardsTriggerScriptId(data.Keys.Contains("receiveRewardsTriggerScriptId") && data["receiveRewardsTriggerScriptId"] != null ? data["receiveRewardsTriggerScriptId"].ToString() : null)
                .WithReceiveRewardsDoneTriggerScriptId(data.Keys.Contains("receiveRewardsDoneTriggerScriptId") && data["receiveRewardsDoneTriggerScriptId"] != null ? data["receiveRewardsDoneTriggerScriptId"].ToString() : null)
                .WithReceiveRewardsDoneTriggerQueueNamespaceId(data.Keys.Contains("receiveRewardsDoneTriggerQueueNamespaceId") && data["receiveRewardsDoneTriggerQueueNamespaceId"] != null ? data["receiveRewardsDoneTriggerQueueNamespaceId"].ToString() : null)
                .WithQueueNamespaceId(data.Keys.Contains("queueNamespaceId") && data["queueNamespaceId"] != null ? data["queueNamespaceId"].ToString() : null)
                .WithKeyId(data.Keys.Contains("keyId") && data["keyId"] != null ? data["keyId"].ToString() : null)
                .WithCompleteNotification(data.Keys.Contains("completeNotification") && data["completeNotification"] != null ? NotificationSetting.FromDict(data["completeNotification"]) : null)
                .WithLogSetting(data.Keys.Contains("logSetting") && data["logSetting"] != null ? LogSetting.FromDict(data["logSetting"]) : null)
                .WithCreatedAt(data.Keys.Contains("createdAt") && data["createdAt"] != null ? (long?)long.Parse(data["createdAt"].ToString()) : null)
                .WithUpdatedAt(data.Keys.Contains("updatedAt") && data["updatedAt"] != null ? (long?)long.Parse(data["updatedAt"].ToString()) : null);
        }
	}
}