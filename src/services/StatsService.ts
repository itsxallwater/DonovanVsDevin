import axios, { AxiosInstance } from "axios";

export default class StatsService {
  private readonly apiClient: AxiosInstance;

  constructor(apiServiceUrl: string) {
    this.apiClient = axios.create({
      baseURL: apiServiceUrl,
      withCredentials: false,
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    });
  }

  // API Client Calls
  async getPER(id: Number) {
    return this.fetch<any>(this.apiClient, `/api/PER?playerId=${id}`);
  }

  // Helpers
  private async fetch<T>(client: AxiosInstance, endpoint: string) {
    return new Promise((resolve, reject) => {
      client
        // .get<T>(endpoint, { headers: { Authorization: `Bearer ${token}` } })
        .get<T>(endpoint)
        .then((resp) => resolve(resp.data))
        .catch((error) => reject(error));
    });
  }
}
