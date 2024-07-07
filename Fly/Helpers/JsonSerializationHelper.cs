using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fly.Models;

namespace Fly.Helpers
{
    internal class JsonSerializationHelper
    {
        public static JsonSerializerOptions JsonSerializerOptions { get; }

        static JsonSerializationHelper()
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new FullCoordinateInformationConverter());
            options.Converters.Add(new MarkerConverter());
            JsonSerializerOptions = options;
        }

        private class FullCoordinateInformationConverter : JsonConverter<FullCoordinateInformationModel>
        {
            public override FullCoordinateInformationModel Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                var result = new FullCoordinateInformationModel();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return result;
                    }

                    // Get the key.
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    string? propertyName = reader.GetString();
                    if (propertyName == null)
                    {
                        throw new JsonException();
                    }
                    else
                    {
                        reader.Read();
                        if (propertyName.Equals(nameof(FullCoordinateInformationModel.City), StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.City = reader.GetString();
                        }
                        else if (propertyName.Equals(nameof(FullCoordinateInformationModel.DisplayName), StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.DisplayName = reader.GetString();
                        }
                        else if (propertyName.Equals(nameof(FullCoordinateInformationModel.Coordinate.Latitude), StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.Coordinate.Latitude = reader.GetDouble();
                        }
                        else if (propertyName.Equals(nameof(FullCoordinateInformationModel.Coordinate.Longitude), StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.Coordinate.Longitude = reader.GetDouble();
                        }
                        else if (propertyName.Equals(nameof(FullCoordinateInformationModel.Elevation), StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (reader.TokenType == JsonTokenType.Number)
                            {
                                result.Elevation = reader.GetDouble();
                            }
                        }
                        // TODO: AirspacesInformation
                        //else if (propertyName.Equals(nameof(CoordinateInformationModel.AirspacesInformation), StringComparison.InvariantCultureIgnoreCase))
                        //{
                        //}
                        else
                        {
                            throw new JsonException();
                        }
                    }
                }

                throw new JsonException();
            }

            public override void Write(
                Utf8JsonWriter writer,
                FullCoordinateInformationModel model,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                if (model.City != null)
                {
                    writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(FullCoordinateInformationModel.City)) ?? nameof(FullCoordinateInformationModel.City));
                    JsonSerializer.Serialize(writer, model.City, options);
                }

                if (model.DisplayName != null)
                {
                    writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(FullCoordinateInformationModel.DisplayName)) ?? nameof(FullCoordinateInformationModel.DisplayName));
                    JsonSerializer.Serialize(writer, model.DisplayName, options);
                }

                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(FullCoordinateInformationModel.Coordinate.Latitude)) ?? nameof(FullCoordinateInformationModel.Coordinate.Latitude));
                JsonSerializer.Serialize(writer, model.Coordinate.Latitude, options);

                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(FullCoordinateInformationModel.Coordinate.Longitude)) ?? nameof(FullCoordinateInformationModel.Coordinate.Longitude));
                JsonSerializer.Serialize(writer, model.Coordinate.Longitude, options);

                if (model.Elevation != null)
                {
                    writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(FullCoordinateInformationModel.Elevation)) ?? nameof(FullCoordinateInformationModel.Elevation));
                    JsonSerializer.Serialize(writer, model.Elevation, options);
                }

                // TODO: AirspacesInformation

                writer.WriteEndObject();
            }
        }

        private class MarkerConverter : JsonConverter<MarkerModel>
        {
            private const string COORDINATE_PROPERTY_NAME = "coordinate";

            public override MarkerModel Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                var result = new MarkerModel();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return result;
                    }

                    // Get the key.
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    string? propertyName = reader.GetString();
                    if (propertyName == null)
                    {
                        throw new JsonException();
                    }
                    else
                    {
                        reader.Read();
                        if (propertyName.Equals(COORDINATE_PROPERTY_NAME, StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.FullCoordinateInformationModel = JsonSerializer.Deserialize<FullCoordinateInformationModel>(ref reader, options);
                        }
                        else if (propertyName.Equals(nameof(MarkerModel.Id), StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.Id = reader.GetInt32();
                        }
                        else if (propertyName.Equals(nameof(MarkerModel.IsVisible), StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.IsVisible = reader.GetBoolean();
                        }
                        else
                        {
                            throw new JsonException();
                        }
                    }
                }

                throw new JsonException();
            }

            public override void Write(
                Utf8JsonWriter writer,
                MarkerModel model,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(COORDINATE_PROPERTY_NAME) ?? COORDINATE_PROPERTY_NAME);
                JsonSerializer.Serialize(writer, model.FullCoordinateInformationModel, options);

                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(MarkerModel.Id)) ?? nameof(MarkerModel.Id));
                JsonSerializer.Serialize(writer, model.Id, options);

                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(nameof(MarkerModel.IsVisible)) ?? nameof(MarkerModel.IsVisible));
                JsonSerializer.Serialize(writer, model.IsVisible, options);

                writer.WriteEndObject();
            }
        }
    }
}
