import axios, { AxiosInstance } from "axios";

export default class StatsService {
  private readonly apiClient: AxiosInstance;
  private readonly apiKey: string;

  constructor(apiServiceUrl: string, apiServiceKey: string) {
    this.apiKey = apiServiceKey;
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
    return this.fetch<any>(
      this.apiClient,
      `/api/PER?playerId=${id}`,
      this.apiKey
    );
  }

  // Helpers
  private async fetch<T>(
    client: AxiosInstance,
    endpoint: string,
    token: string
  ) {
    return new Promise((resolve, reject) => {
      client
        // .get<T>(endpoint, { headers: { Authorization: `Bearer ${token}` } })
        .get<T>(`${endpoint}&code=${token}`)
        .then((resp) => resolve(resp.data))
        .catch((error) => reject(error));
    });
  }
}
