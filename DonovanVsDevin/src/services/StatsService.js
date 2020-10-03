import axios from "axios";
export default class StatsService {
    constructor(apiServiceUrl, apiServiceKey) {
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
    async getPER(id) {
        return this.fetch(this.apiClient, `PER?playerId=${id}`, this.apiKey);
    }
    // Helpers
    async fetch(client, endpoint, token) {
        return new Promise((resolve, reject) => {
            client
                // .get<T>(endpoint, { headers: { Authorization: `Bearer ${token}` } })
                .get(`${endpoint}&code=${token}`)
                .then((resp) => resolve(resp.data))
                .catch((error) => reject(error));
        });
    }
}
