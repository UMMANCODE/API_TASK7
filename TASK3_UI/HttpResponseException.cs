namespace TASK3_UI {
  public class HttpResponseException : Exception {
    public HttpResponseMessage Response { get; }
    public HttpResponseException(HttpResponseMessage response) : base($"HTTP Error: {response.StatusCode}") {
      Response = response;
    }
  }

}
