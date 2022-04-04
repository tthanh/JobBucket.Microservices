namespace JB.Job.Constants
{
    public enum InterviewStatus
    {
		Unverified = 0, // đang chờ ứng viên xác nhận lịch pv

		Accepted = 1, //uv chấp nhận lịch
		Denied = 2, //uv ko chấp nhận lịch ->  hr chọn reschedule -> Unverified


		Passed = 3, // đậu vòng hiện tại
		Failed = 4  // fail vòng hiện tại -> application failed

	}
}
