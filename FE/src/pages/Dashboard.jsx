import { useEffect, useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { productService } from '../api/productService'
import { storageService } from '../api/storageService'
import { productTypeService } from '../api/productTypeService'
import { storageTypeService } from '../api/storageTypeService'
import { userService } from '../api/userService'
import { useAuth } from '../contexts/AuthContext'
import { logError } from '../utils/errorHandler'
import { normalizeResponse, normalizeArray } from '../utils/dataNormalizer'
import {
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  LineChart,
  Line
} from 'recharts'
import './Dashboard.css'

const Dashboard = () => {
  const { user: currentUser } = useAuth()
  const navigate = useNavigate()
  const [stats, setStats] = useState({
    totalProducts: 0,
    totalStorage: 0,
    totalProductTypes: 0,
    totalStorageTypes: 0,
    totalQuantity: 0,
    totalUsers: 0,
    loading: true,
  })
  const [chartData, setChartData] = useState({
    products: [],
    storages: [],
    productTypes: [],
    storageTypes: [],
  })

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const promises = [
          productService.getAll({ pageSize: 1000 }), // Lấy tất cả để tính toán biểu đồ
          storageService.getAll(),
          productTypeService.getAll(),
          storageTypeService.getAll(),
        ]

        // Chỉ fetch users nếu là Admin hoặc Manager
        if (currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')) {
          promises.push(userService.getAll())
        }

        const results = await Promise.all(promises)

        const productsData = normalizeResponse(results[0])
        const products = productsData.items || []
        const totalProducts = productsData.totalItems || 0
        
        const storages = normalizeArray(results[1])
        const totalStorage = storages.length
        const totalQuantity = storages.reduce((sum, s) => sum + (s.quantity || 0), 0)
        
        const productTypes = normalizeArray(results[2])
        const storageTypes = normalizeArray(results[3])

        // Users (nếu có)
        let totalUsers = 0
        if (results[4]) {
          const users = normalizeArray(results[4])
          totalUsers = users.length
        }

        setStats({
          totalProducts,
          totalStorage,
          totalProductTypes: productTypes.length,
          totalStorageTypes: storageTypes.length,
          totalQuantity,
          totalUsers,
          loading: false,
        })

        setChartData({
          products,
          storages,
          productTypes,
          storageTypes,
        })
      } catch (error) {
        logError(error, 'Dashboard.fetchStats')
        setStats((prev) => ({ ...prev, loading: false }))
      }
    }

    fetchStats()
  }, [currentUser])

  // Tính toán dữ liệu cho biểu đồ - PHẢI ĐẶT TRƯỚC ĐIỀU KIỆN RETURN
  const productTypeChartData = useMemo(() => {
    const typeMap = new Map()
    chartData.products.forEach(product => {
      const typeName = product.productTypeName || 'Chưa phân loại'
      typeMap.set(typeName, (typeMap.get(typeName) || 0) + 1)
    })
    return Array.from(typeMap.entries()).map(([name, count]) => ({
      name,
      value: count
    }))
  }, [chartData.products])

  const storageTypeChartData = useMemo(() => {
    const typeMap = new Map()
    chartData.storages.forEach(storage => {
      const typeName = storage.storageTypeName || 'Chưa phân loại'
      const quantity = storage.quantity || 0
      typeMap.set(typeName, (typeMap.get(typeName) || 0) + quantity)
    })
    return Array.from(typeMap.entries()).map(([name, value]) => ({
      name,
      value
    }))
  }, [chartData.storages])

  const topStoragesChartData = useMemo(() => {
    return chartData.storages
      .map(storage => ({
        name: storage.productName || `Sản phẩm #${storage.productId}`,
        quantity: storage.quantity || 0
      }))
      .sort((a, b) => b.quantity - a.quantity)
      .slice(0, 5)
  }, [chartData.storages])

  const monthlyImportData = useMemo(() => {
    const monthMap = new Map()
    chartData.storages.forEach(storage => {
      const importDate = storage.importDate
      if (importDate) {
        const date = new Date(importDate)
        const monthKey = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`
        const monthLabel = `${date.getMonth() + 1}/${date.getFullYear()}`
        monthMap.set(monthKey, {
          month: monthLabel,
          quantity: (monthMap.get(monthKey)?.quantity || 0) + (storage.quantity || 0)
        })
      }
    })
    return Array.from(monthMap.values())
      .sort((a, b) => {
        const [monthA, yearA] = a.month.split('/').map(Number)
        const [monthB, yearB] = b.month.split('/').map(Number)
        if (yearA !== yearB) return yearA - yearB
        return monthA - monthB
      })
      .slice(-6) // Lấy 6 tháng gần nhất
  }, [chartData.storages])

  const COLORS = ['#3498db', '#9b59b6', '#e67e22', '#16a085', '#27ae60', '#c0392b', '#f39c12', '#1abc9c']

  if (stats.loading) {
    return (
      <div className="dashboard">
        <div className="loading-skeleton">
          <div className="skeleton-card"></div>
          <div className="skeleton-card"></div>
          <div className="skeleton-card"></div>
          <div className="skeleton-card"></div>
          <div className="skeleton-card"></div>
          {currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager') ? (
            <div className="skeleton-card"></div>
          ) : null}
        </div>
      </div>
    )
  }

  const statCards = [
    {
      icon: '📦',
      title: 'Tổng sản phẩm',
      value: stats.totalProducts,
      color: '#3498db',
      bgColor: '#ebf5fb',
      path: '/products'
    },
    {
      icon: '🏷️',
      title: 'Loại sản phẩm',
      value: stats.totalProductTypes,
      color: '#9b59b6',
      bgColor: '#f4ecf7',
      path: '/product-types'
    },
    {
      icon: '🏪',
      title: 'Tổng kho',
      value: stats.totalStorage,
      color: '#e67e22',
      bgColor: '#fdf2e9',
      path: '/storage'
    },
    {
      icon: '📋',
      title: 'Loại kho',
      value: stats.totalStorageTypes,
      color: '#16a085',
      bgColor: '#e8f8f5',
      path: '/storage-types'
    },
    {
      icon: '📊',
      title: 'Tổng số lượng',
      value: stats.totalQuantity.toLocaleString('vi-VN'),
      color: '#27ae60',
      bgColor: '#eafaf1',
      path: '/storage' // Điều hướng đến trang kho để xem chi tiết
    }
  ]

  // Thêm thống kê users nếu là Admin hoặc Manager
  if (currentUser?.roles?.includes('Admin') || currentUser?.roles?.includes('Manager')) {
    statCards.push({
      icon: '👥',
      title: 'Tổng người dùng',
      value: stats.totalUsers,
      color: '#c0392b',
      bgColor: '#fadbd8',
      path: '/users'
    })
  }

  const handleCardClick = (path) => {
    if (path) {
      navigate(path)
    }
  }

  return (
    <div className="dashboard">
      <div className="dashboard-header">
        <h1>Bảng điều khiển</h1>
        <p className="dashboard-subtitle">Tổng quan hệ thống quản lý kho</p>
      </div>

      <div className="stats-grid">
        {statCards.map((card, index) => (
          <div 
            key={index} 
            className="stat-card clickable"
            style={{ 
              borderTop: `4px solid ${card.color}`,
              backgroundColor: card.bgColor
            }}
            onClick={() => handleCardClick(card.path)}
            role="button"
            tabIndex={0}
            onKeyDown={(e) => {
              if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault()
                handleCardClick(card.path)
              }
            }}
          >
            <div className="stat-icon" style={{ color: card.color }}>
              {card.icon}
            </div>
            <div className="stat-content">
              <h3>{card.title}</h3>
              <p className="stat-value" style={{ color: card.color }}>
                {card.value}
              </p>
            </div>
            <div className="stat-arrow">→</div>
          </div>
        ))}
      </div>

      {/* Charts Section */}
      <div className="charts-section">
        <div className="chart-row">
          <div className="chart-card">
            <h3 className="chart-title">Phân bố sản phẩm theo loại</h3>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={productTypeChartData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" angle={-45} textAnchor="end" height={80} />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="value" fill="#3498db" name="Số lượng" />
              </BarChart>
            </ResponsiveContainer>
          </div>

          <div className="chart-card">
            <h3 className="chart-title">Phân bố số lượng theo loại kho</h3>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={storageTypeChartData}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                  outerRadius={100}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {storageTypeChartData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        <div className="chart-row">
          <div className="chart-card">
            <h3 className="chart-title">Top 5 kho có số lượng nhiều nhất</h3>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={topStoragesChartData} layout="vertical">
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis type="number" />
                <YAxis dataKey="name" type="category" width={120} />
                <Tooltip />
                <Legend />
                <Bar dataKey="quantity" fill="#27ae60" name="Số lượng" />
              </BarChart>
            </ResponsiveContainer>
          </div>

          <div className="chart-card">
            <h3 className="chart-title">Xu hướng nhập kho theo tháng</h3>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={monthlyImportData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey="quantity" stroke="#e67e22" strokeWidth={2} name="Số lượng nhập" />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      <div className="welcome-section">
        <div className="welcome-header">
          <h2>Chào mừng, {currentUser?.userName || 'Người dùng'}!</h2>
          <p className="welcome-role">
            Vai trò: <strong>{currentUser?.roles?.[0] || 'User'}</strong>
          </p>
        </div>
        <div className="welcome-content">
          <p>Sử dụng menu bên trái để điều hướng đến các chức năng quản lý.</p>
          <div className="quick-info">
            <div className="info-icon">ℹ️</div>
            <div className="info-content">
              <p><strong>Hệ thống quản lý kho</strong> giúp bạn quản lý sản phẩm, kho hàng và người dùng một cách hiệu quả.</p>
              <ul className="info-list">
                <li>Quản lý sản phẩm và loại sản phẩm</li>
                <li>Theo dõi kho hàng và số lượng tồn kho</li>
                <li>Quản lý người dùng và phân quyền</li>
                <li>Báo cáo và thống kê chi tiết</li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Dashboard

