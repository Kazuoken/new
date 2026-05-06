import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || '',
  headers: { 'Content-Type': 'application/json' }
})

export const urlApi = {
  create: (data) => api.post('/api/url', data).then(r => r.data),
  getAll: (page = 1, pageSize = 20) =>
    api.get(`/api/url/manage?page=${page}&pageSize=${pageSize}`).then(r => r.data),
  getById: (id) => api.get(`/api/url/${id}`).then(r => r.data),
  update: (id, data) => api.patch(`/api/url/${id}`, data).then(r => r.data),
  delete: (id) => api.delete(`/api/url/${id}`),
}
