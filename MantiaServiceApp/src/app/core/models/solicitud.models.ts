export type OrigenSolicitud = 'Correo' | 'Mensajeria' | 'PDF';

export type EstadoSolicitud =
  | 'ListaParaCotizar'
  | 'FaltaInformacion'
  | 'RequiereValoracionTecnica';

export interface ItemSolicitud {
  codigoTarifa: string;
  concepto: string;
  cantidad: number;
  observacion: string;
}

export interface SolicitudListado {
  id: number;
  numero: string;
  fechaRecepcion: string;
  cliente: string;
  municipio: string;
  origen: OrigenSolicitud;
  estado: EstadoSolicitud;
  referencia: string;
  camposFaltantes: string;
}

export interface SolicitudDetalle {
  id: number;
  numero: string;
  fechaRecepcion: string;
  cliente: string;
  contacto: string;
  municipio: string;
  direccion: string;
  emailContacto: string;
  origen: OrigenSolicitud;
  referencia: string;
  textoOriginal: string;
  estado: EstadoSolicitud;
  camposFaltantes: string;
  observaciones: string;
  fechaVisita: string | null;
  franjaHoraria: string;
  finDeSemana: boolean;
  fueraDeHorario: boolean;
  items: ItemSolicitud[];
}

export interface AnalizarTextoRequest {
  texto: string;
  origen: OrigenSolicitud;
}

export interface ActualizarSolicitudRequest {
  cliente?: string | null;
  contacto?: string | null;
  municipio?: string | null;
  direccion?: string | null;
  emailContacto?: string | null;
  referencia?: string | null;
  fechaVisita?: string | null;
  franjaHoraria?: string | null;
  estado?: EstadoSolicitud | null;
  observaciones?: string | null;
  items?: ItemSolicitud[] | null;
}

export interface ResultadoExtraccion {
  motor: string;
  cliente: string;
  contacto: string;
  municipio: string;
  direccion: string;
  emailContacto: string;
  referencia: string;
  fechaVisita: string | null;
  franjaHoraria: string;
  finDeSemana: boolean;
  fueraDeHorario: boolean;
  estado: EstadoSolicitud;
  camposFaltantes: string;
  observaciones: string;
  items: ItemSolicitud[];
}

export const ESTADOS_SOLICITUD: EstadoSolicitud[] = [
  'ListaParaCotizar',
  'FaltaInformacion',
  'RequiereValoracionTecnica',
];

export const ORIGENES_SOLICITUD: OrigenSolicitud[] = ['Correo', 'Mensajeria', 'PDF'];
