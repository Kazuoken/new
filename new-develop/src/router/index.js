import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import StaticTableView from '../views/StaticView.vue'

const routes = [
  {
    path: '/',
    name: 'home',
    component: HomeView
  },
  {
    path: '/table',
    name: 'table',
    component: StaticTableView
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router