import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import api from '../services/api';

export interface Source {
  id: number;
  name: string;
}

export interface Prospect {
  id: number;
  playerName: string;
  team: string;
  position: string;
  age: number;
  eta: string | null;
  rank: number;
  sourceId: number;
  source: Source;
}

interface ProspectsState {
  prospects: Prospect[];
  sources: Source[];
  isLoading: boolean;
  error: string | null;
}

const initialState: ProspectsState = {
  prospects: [],
  sources: [],
  isLoading: false,
  error: null,
};

export const fetchProspects = createAsyncThunk(
  'prospects/fetchAll',
  async (sourceId?: number) => {
    const params = sourceId ? { sourceId } : {};
    const response = await api.get<Prospect[]>('/prospects', { params });
    return response.data;
  }
);

export const fetchSources = createAsyncThunk(
  'prospects/fetchSources',
  async () => {
    const response = await api.get<Source[]>('/sources');
    return response.data;
  }
);

const prospectsSlice = createSlice({
  name: 'prospects',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchProspects.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchProspects.fulfilled, (state, action) => {
        state.isLoading = false;
        state.prospects = action.payload;
      })
      .addCase(fetchProspects.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to fetch prospects';
      })
      .addCase(fetchSources.fulfilled, (state, action) => {
        state.sources = action.payload;
      });
  },
});

export default prospectsSlice.reducer;
