using System.Collections.Generic;
using Newtonsoft.Json;

namespace HwModule.Core
{
    /// <summary>
    /// HwModule v3 통합 프린트 데이터 모델
    /// 엔드포인트: /print, /receipt, /ticket
    /// </summary>
    public class V3PrintData
    {
        [JsonProperty("version")]
        public string Version { get; set; } = "v3";

        /// <summary>receipt | ticket | both</summary>
        [JsonProperty("printType")]
        public string PrintType { get; set; } = "receipt";

        [JsonProperty("facility")]
        public V3Facility Facility { get; set; }

        [JsonProperty("reservation")]
        public V3Reservation Reservation { get; set; }

        [JsonProperty("payment")]
        public V3Payment Payment { get; set; }

        [JsonProperty("items")]
        public List<V3Item> Items { get; set; }

        /// <summary>발권 시점 Unix timestamp (ms)</summary>
        [JsonProperty("printedAt")]
        public long PrintedAt { get; set; }
    }

    public class V3Facility
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("busiNo")]
        public string BusiNo { get; set; }

        [JsonProperty("ceoName")]
        public string CeoName { get; set; }

        [JsonProperty("tel")]
        public string Tel { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>영수증 하단 문구 (빈 문자열이면 미출력)</summary>
        [JsonProperty("receiptFooter")]
        public string ReceiptFooter { get; set; } = "";

        /// <summary>티켓 한줄 알림 (빈 문자열이면 미출력)</summary>
        [JsonProperty("ticketFooter")]
        public string TicketFooter { get; set; } = "";
    }

    public class V3Reservation
    {
        [JsonProperty("orderNum")]
        public string OrderNum { get; set; }

        [JsonProperty("guestName")]
        public string GuestName { get; set; }

        /// <summary>뒷 4자리 마스킹된 연락처 (예: 010-1234-****)</summary>
        [JsonProperty("guestPhone")]
        public string GuestPhone { get; set; }

        /// <summary>차량번호 (없으면 빈 문자열)</summary>
        [JsonProperty("carNum")]
        public string CarNum { get; set; } = "";

        /// <summary>이용 시작일 (yyyyMMdd)</summary>
        [JsonProperty("useDate")]
        public string UseDate { get; set; }

        /// <summary>이용 종료일 (yyyyMMdd, 1박 이상 시 사용)</summary>
        [JsonProperty("useDateEnd")]
        public string UseDateEnd { get; set; } = "";

        /// <summary>대표 구역명</summary>
        [JsonProperty("areaName")]
        public string AreaName { get; set; }
    }

    public class V3Payment
    {
        /// <summary>CARD | CASH</summary>
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("cardName")]
        public string CardName { get; set; } = "";

        [JsonProperty("maskingNum")]
        public string MaskingNum { get; set; } = "";

        [JsonProperty("authCode")]
        public string AuthCode { get; set; } = "";

        /// <summary>14자리 (yyyyMMddHHmmss)</summary>
        [JsonProperty("authDate")]
        public string AuthDate { get; set; } = "";

        /// <summary>할부개월 (00=일시불)</summary>
        [JsonProperty("quota")]
        public string Quota { get; set; } = "00";

        [JsonProperty("totalAmt")]
        public long TotalAmt { get; set; }

        [JsonProperty("dcAmt")]
        public long DcAmt { get; set; }

        [JsonProperty("vatAmt")]
        public long VatAmt { get; set; }

        [JsonProperty("totalPay")]
        public long TotalPay { get; set; }
    }

    public class V3Item
    {
        [JsonProperty("areaName")]
        public string AreaName { get; set; }

        [JsonProperty("roomName")]
        public string RoomName { get; set; }

        /// <summary>이용일 (yyyyMMdd)</summary>
        [JsonProperty("useDate")]
        public string UseDate { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("qty")]
        public int Qty { get; set; } = 1;
    }
}
